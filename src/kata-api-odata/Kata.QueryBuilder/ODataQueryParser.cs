using ComplexFilters.Abstractions.Models;
using Kata.QueryBuilder.Clause;
using Kata.QueryBuilder.ComplexFilters;
using Kata.QueryBuilder.Models;
using Microsoft.OData.UriParser;
using SQL.ReadModel.Context;
using SqlKata;
using SqlKata.Compilers;

namespace Kata.QueryBuilder
{

    public class ODataQueryParser : IODataQueryParser
    {
        private readonly SqlServerCompiler _sqlKataCompiler;
        private readonly IReaderDapperContext _readContext;
        private readonly IEdmModelBuilder _edmModelBuilder;
        private readonly IKataComplexFilterManager _kataComplexFilterManager;

        private const string CUSTOM_SCHEMA = "custom";

        public ODataQueryParser(IReaderDapperContext readContext,
                                IEdmModelBuilder edmModelBuilder,
                                IKataComplexFilterManager kataComplexFilterManager)
        {
            _sqlKataCompiler = new SqlServerCompiler { UseLegacyPagination = false };
            _readContext = readContext;
            _edmModelBuilder = edmModelBuilder;
            _kataComplexFilterManager = kataComplexFilterManager;
        }

        public async Task<ODataQueryResult> ExecuteQuery<T>(Dictionary<string, string> options, string schema, string tableName,
            IEnumerable<ComplexFilter>? defaultFilters = default) where T : class
        {
            ODataQueryResult oDataQueryResult = new();

            string entityName = await GetEntityName(schema, tableName); //ritorna il nome dell'entità su cui eseguire la query

            var parser = await CreateEdmModel(entityName, options);

            var queries = await BuildQuery<T>(parser, entityName, defaultFilters);

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

        public async Task<Query> CreateQuery<T>(Dictionary<string, string> options, string schema, string tableName,
            IEnumerable<ComplexFilter>? defaultFilters = default) where T : class
        {
            //ritorna l'oggetto Query, il quale può essere alterato, aggiungendo condizioni, prima di eseguire le query vera e propria
            string entityName = await GetEntityName(schema, tableName); //ritorna il nome dell'entità su cui eseguire la query

            ODataQueryOptionParser parser = await CreateEdmModel(entityName, options);

            var query = await BuildQueryCore<T>(parser, entityName, defaultFilters);

            return query;
        }

        public async Task<List<SqlQueryToExecute>> GetQueryToExecuteAsync(Query queryIn)
        {
            //ritorna le query da eseguire
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
                    //ritorna la query da eseguire per avere il count dei record 
                    var clonedQuery = queryIn.Clone();
                    queryToExecute = CompileSqlKataQuery(clonedQuery.AsCount(), true);
                }
                else
                {
                    //ritorna la query da eseguire per ottenere i dati
                    queryToExecute = CompileSqlKataQuery(queryIn);
                }

                if (queryToExecute != null)
                {
                    //comando sql
                    sqlDapperQuery.SqlCommand = queryToExecute.TSqlQuery;

                    //parametri                    
                    sqlDapperQuery.Parameters = new Dapper.DynamicParameters(queryToExecute.Parameters);
                }

                return sqlDapperQuery;
            });
        }


        public async Task<ODataQueryResult> ExecQuery(Query queryIn)
        {
            //esegue la query odata

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

        private async Task<string> GetEntityName(string schema, string tableName)
        {
            //controllo esistenza vista custom                     

            Query q = new Query("INFORMATION_SCHEMA.VIEWS");
            q.Where("TABLE_NAME", $"'{tableName}'");
            q.Where("TABLE_SCHEMA", $"'{CUSTOM_SCHEMA}'");

            SqlQueryToExecute sqlQuery = CompileSqlKataQuery(q.AsCount(), true);

            int count = await _readContext.CountAsync(sqlQuery.TSqlQuery, sqlQuery.Parameters);
            if (count > 0)
            {
                schema = CUSTOM_SCHEMA;
            }

            return $"{schema}.{tableName}";
        }

        private async Task<IList<SqlQueryToExecute>> BuildQuery<T>(ODataQueryOptionParser parser, string tableName,
                                                                    IEnumerable<ComplexFilter>? defaultFilters = default) where T : class
        {

            Query query = await BuildQueryCore<T>(parser, tableName, defaultFilters);

            List<SqlQueryToExecute> queries = new();

            var clonedQuery = query.Clone();
            queries.Add(CompileSqlKataQuery(clonedQuery.AsCount(), true));
            queries.Add(CompileSqlKataQuery(query));

            return queries;
        }

        private async Task<Query> BuildQueryCore<T>(ODataQueryOptionParser parser, string tableName,
                                                IEnumerable<ComplexFilter>? defaultFilters = default) where T : class
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

            query = await _kataComplexFilterManager.ApplyFilters<T>(query, tableName, defaultFilters);

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

        private static Query BuildWhereClause(Query query, IEnumerable<WhereFilter> whereFilters)
        {
            //crea where 

            foreach (var filter in whereFilters)
            {
                query.Clauses.AddRange(query.Clauses);
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
