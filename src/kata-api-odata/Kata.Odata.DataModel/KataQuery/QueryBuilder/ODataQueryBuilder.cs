using Dapper;
using Kata.Odata.DataModel.KataQuery.EdmModel;
using Kata.Odata.DataModel.KataQuery.Models;
using Kata.Odata.DataModel.KataQuery.QueryBuilder;
using Microsoft.OData.UriParser;
using SqlKata;
using SqlKata.Compilers;


namespace Kata.Odata.DataModel.KataQuery
{

    public class ODataQueryBuilder : IODataQueryBuilder
    {
        private readonly SqlServerCompiler _sqlKataCompiler;
        private readonly IEdmModelBuilder _edmModelBuilder;

        public ODataQueryBuilder(IEdmModelBuilder edmModelBuilder)
        {
            _sqlKataCompiler = new SqlServerCompiler { UseLegacyPagination = false };
            _edmModelBuilder = edmModelBuilder;
        }



        public async Task<Query> CreateQuery<T>(Dictionary<string, string> options, string viewName)
            where T : class
        {
            QueryInfo info = await CreateQueryCore<T>(options, viewName);
            return info.Query ?? throw new ArgumentNullException("Cannot generate any Kata Query");
        }


        private async Task<QueryInfo> CreateQueryCore<T>(Dictionary<string, string> options, string viewName)
           where T : class
        {
            QueryInfo info = new QueryInfo
            {
                EntityName = viewName
            };

            ODataQueryOptionParser parser = await CreateEdmModel(info.EntityName, options);

            info.Query = await BuildQueryCore<T>(parser, info.EntityName);

            return info;
        }



        public async Task<Query> CreateQuery<T>(Dictionary<string, string> options, string viewName, IEnumerable<int> aOrgs)
        where T : class
        {
            QueryInfo info = await CreateQueryCore<T>(options, viewName);
            return info.Query ?? throw new ArgumentNullException("Cannot generate any Kata Query");
        }


        public async Task<List<BuilderQueryResult>> GetQueryToExecuteAsync(Query queryIn)
        {
            return await Task.Run(() =>
             {
                 List<BuilderQueryResult> queries = new();

                 var clonedQuery = queryIn.Clone();

                 queries.Add(CompileSqlKataQuery(clonedQuery.AsCount(), true));
                 queries.Add(CompileSqlKataQuery(queryIn));

                 return queries;
             });
        }

        public async Task<SqlQuery> GetSqlQuery(Query queryIn, bool includeCount = false)
        {
            return await Task.Run(() =>
            {
                SqlQuery sqlDapperQuery = new();
                BuilderQueryResult? queryToExecute = null;

                if (includeCount)
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

        private async Task<IList<BuilderQueryResult>> BuildQuery<T>(ODataQueryOptionParser parser, string tableName)
            where T : class
        {

            Query query = await BuildQueryCore<T>(parser, tableName);

            List<BuilderQueryResult> queries = new();

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

        private BuilderQueryResult CompileSqlKataQuery(Query query, bool isCount = false)
        {
            var sqlResult = _sqlKataCompiler.Compile(query);
            return new BuilderQueryResult() { IsCountQuery = isCount, TSqlQuery = sqlResult.Sql, Parameters = sqlResult.NamedBindings };
        }

    }


}
