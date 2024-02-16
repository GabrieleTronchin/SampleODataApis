using Dapper;
using Kata.Odata.DataModel.DataReader;
using Kata.Odata.DataModel.KataQuery.Clause;
using Kata.Odata.DataModel.KataQuery.Models;
using Microsoft.OData.UriParser;
using SqlKata;
using SqlKata.Compilers;

namespace Kata.Odata.DataModel.KataQuery
{

    public class ODataQueryParser : IODataQueryParser
    {
        private readonly SqlServerCompiler _sqlKataCompiler;
        private readonly IEdmModelBuilder _edmModelBuilder;
        private readonly IDataReader _readContext;

        public ODataQueryParser(IEdmModelBuilder edmModelBuilder, IDataReader dapper)
        {
            _sqlKataCompiler = new SqlServerCompiler { UseLegacyPagination = false };
            _edmModelBuilder = edmModelBuilder;
            _readContext = dapper;
        }

        public async Task<ODataQueryResult> ExecuteQuery<T>(Dictionary<string, string> options, string schema, string tableName) where T : class
        {
            ODataQueryResult oDataQueryResult = new();

            string entityName = $"{schema}.{tableName}";

            var parser = await CreateEdmModel(entityName, options);

            var queries = await BuildQuery<T>(parser, entityName);

            foreach (var query in queries)
            {
                if (query.IsCountQuery)
                {
                    oDataQueryResult.Count = await _readContext.CountAsync(query.TSqlQuery, query.Parameters);
                }
                else
                {
                    oDataQueryResult.Value = await _readContext.QueryAsync(query.TSqlQuery, query.Parameters);
                }
            }

            return oDataQueryResult;
        }

        public async Task<Query> CreateQuery<T>(Dictionary<string, string> options, string schema, string tableName) where T : class
        {
            string entityName = $"{schema}.{tableName}";

            ODataQueryOptionParser parser = await CreateEdmModel(entityName, options);

            var query = await BuildQueryCore<T>(parser, entityName);

            return query;
        }

        public async Task<List<SqlQueryToExecute>> GetQueryToExecuteAsync(Query queryIn)
        {
            return await Task.Run(() =>
             {
                 List<SqlQueryToExecute> queries = new();

                 var clonedQuery = queryIn.Clone();

                 queries.Add(CompileSqlKataQuery(clonedQuery.AsCount(), true));
                 queries.Add(CompileSqlKataQuery(queryIn));

                 return queries;
             });
        }

        public async Task<SqlDapperQuery> GetSqlQuery(Query queryIn, bool bCount = false)
        {
            return await Task.Run(() =>
            {
                SqlDapperQuery sqlDapperQuery = new SqlDapperQuery();

                SqlQueryToExecute? queryToExecute = null;

                if (bCount)
                {
                    var clonedQuery = queryIn.Clone();
                    queryToExecute = CompileSqlKataQuery(clonedQuery.AsCount(), true);
                }
                else
                {
                    queryToExecute = CompileSqlKataQuery(queryIn);
                }

                if (queryToExecute != null)
                {
                    sqlDapperQuery.SqlCommand = queryToExecute.TSqlQuery;
                    sqlDapperQuery.Parameters = new DynamicParameters(queryToExecute.Parameters);
                }

                return sqlDapperQuery;
            });
        }

        public async Task<ODataQueryResult> ExecQuery(Query queryIn)
        {
            ODataQueryResult oDataQueryResult = new();

            List<SqlQueryToExecute> queries = await GetQueryToExecuteAsync(queryIn);

            foreach (var query in queries)
            {
                if (query.IsCountQuery)
                {
                    oDataQueryResult.Count = await _readContext.CountAsync(query.TSqlQuery, query.Parameters);
                }
                else
                {
                    oDataQueryResult.Value = await _readContext.QueryAsync(query.TSqlQuery, query.Parameters);
                }
            }

            return oDataQueryResult;
        }

        private async Task<ODataQueryOptionParser> CreateEdmModel(string tableName, Dictionary<string, string> options)
        {
            var result = _edmModelBuilder.BuildTableModel(tableName);
            var model = result.Item1;
            var entityType = result.Item2;
            var entitySet = result.Item3;
            var parser = new ODataQueryOptionParser(model, entityType, entitySet, options);
            parser.Resolver.EnableCaseInsensitive = true;
            parser.Resolver.EnableNoDollarQueryOptions = true;

            return parser;
        }

        private async Task<IList<SqlQueryToExecute>> BuildQuery<T>(ODataQueryOptionParser parser, string tableName) where T : class
        {

            Query query = await BuildQueryCore<T>(parser, tableName);

            List<SqlQueryToExecute> queries = new();

            var clonedQuery = query.Clone();
            queries.Add(CompileSqlKataQuery(clonedQuery.AsCount(), true));
            queries.Add(CompileSqlKataQuery(query));

            return queries;
        }

        private async Task<Query> BuildQueryCore<T>(ODataQueryOptionParser parser, string tableName) where T : class
        {

            var applyClause = parser.ParseApply();
            var filterClause = parser.ParseFilter();
            var top = parser.ParseTop();
            var skip = parser.ParseSkip();
            var orderbyClause = parser.ParseOrderBy();
            var selectClause = parser.ParseSelectAndExpand();

            var query = new Query($"{tableName} as target");

            if (applyClause != null)
            {
                query = new ApplyClauseBuilder().BuildApplyClause(query, applyClause, true);
                if (filterClause != null || selectClause != null)
                {
                    query = new Query().From(query);
                }
            }

            if (filterClause != null)
            {
                query = filterClause.Expression.Accept(new FilterClauseBuilder(query, true));
            }

            if (top != null)
            {
                if (!int.TryParse(top.ToString(), out var iTop))
                {
                    throw new InvalidCastException($"{nameof(top)} is not in a valid format.");
                }
                query = query.Take(iTop);
            }

            if (skip != null)
            {
                if (!int.TryParse(skip.ToString(), out var iSkip))
                {
                    throw new InvalidCastException($"{nameof(skip)} is not in a valid format.");
                }
                query = query.Skip(iSkip);
            }

            if (orderbyClause != null)
            {
                query = BuildOrderByClause(query, orderbyClause);
            }

            if (selectClause != null)
            {
                query = BuildSelectClause(query, selectClause);
            }

            return query;
        }

        private static Query BuildSelectClause(Query query, SelectExpandClause selectClause)
        {
            if (!selectClause.AllSelected)
            {
                foreach (var selectItem in selectClause.SelectedItems)
                {
                    if (selectItem is PathSelectItem path)
                    {
                        query = query.Select(path.SelectedPath.FirstSegment.Identifier.Trim());
                    }
                }
            }

            return query;
        }
        private static Query BuildOrderByClause(Query query, OrderByClause orderbyClause)
        {
            while (orderbyClause != null)
            {
                var direction = orderbyClause.Direction;
                if (orderbyClause.Expression is SingleValueOpenPropertyAccessNode expression)
                {
                    if (direction == OrderByDirection.Ascending)
                    {
                        query = query.OrderBy(expression.Name.Trim());
                    }
                    else
                    {
                        query = query.OrderByDesc(expression.Name.Trim());
                    }
                }

                orderbyClause = orderbyClause.ThenBy;
            }

            return query;
        }

        private SqlQueryToExecute CompileSqlKataQuery(Query query, bool isCount = false)
        {
            var sqlResult = _sqlKataCompiler.Compile(query);
            return new SqlQueryToExecute() { IsCountQuery = isCount, TSqlQuery = sqlResult.Sql, Parameters = sqlResult.NamedBindings };
        }

    }
}
