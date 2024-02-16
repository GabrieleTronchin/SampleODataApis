using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;
using HttpDecorator;
using Kata.QueryBuilder.ComplexFilters.FilterComparators;
using Kata.QueryBuilder.ComplexFilters.Mapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceCache;
using SqlKata;
using System.Linq.Dynamic.Core;

namespace Kata.QueryBuilder.ComplexFilters
{
    public class KataComplexFilterManager : IKataComplexFilterManager
    {
        private readonly ILogger<KataComplexFilterManager> _logger;
        private readonly ICacheService _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly IComplexFilterMapper<ComplexFilterMapper> _mapper;
        private readonly IHttpDecorator _httpDecorator;

        public KataComplexFilterManager(ILogger<KataComplexFilterManager> logger,
                              ICacheService cache,
                              IHttpDecorator httpDecorator,
                              IComplexFilterMapper<ComplexFilterMapper> mapper,
                              IServiceProvider serviceProvider)
        {
            _logger = logger;
            _cache = cache;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _httpDecorator = httpDecorator;
        }


        private string GetCacheKey(string targetEntity, string sessionId)
        {
            return $"{targetEntity}-{sessionId}";
        }


        public async Task<List<ComplexFilter>> GetFilters(string targetEntity)
        {
            var sessionId = _httpDecorator.GetSessionId();

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return new List<ComplexFilter>();
            }

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new InvalidDataException($"{nameof(targetEntity)} must have a value");
            }

            return await _cache.GetOrCreateAsync(
                GetCacheKey(targetEntity.ToLower(), sessionId),
                () => LoadFilterFromCfg(targetEntity)
                ) ?? new List<ComplexFilter>();
        }

        internal async Task<List<ComplexFilter>> LoadFilterFromCfg(string targetEntity)
        {
            //TODO: la libreria generica non può aver riferimenti al Dictionary.

            //var bearerToken = _httpDecorator.GetBearerToken() ?? throw new ArgumentNullException($"{nameof(_httpDecorator.GetBearerToken)}");
            //var appInfo = _httpDecorator.GetAppInfo();

            //_userSettingsHttpClient.SetBearerAuthentication(bearerToken);
            //_userSettingsHttpClient.SetProductId(appInfo.Product);
            //_userSettingsHttpClient.SetAppId(appInfo.AppId ?? "");

            //var entitySavedFilters = await _userSettingsHttpClient.GetFilterAsync<List<Dictionary.Shared.Models.UserSettings.ComplexFilter>>(targetEntity);

            //return _mapper.Mapper.Map<List<ComplexFilter>>(entitySavedFilters);

            return new List<ComplexFilter>();
        }

        private Query ComposeWhereExpression(Query entity, List<ComplexFilter> filters)
        {
            //TODO
            FilterBuilder filterBuilder = new();
            foreach (var filter in filters)
            {
                foreach (var filterValues in filter.EntityFilter)
                {
                    if (filterValues.FilterJoiner) continue;

                    var eFilter = filterValues;

                    if (!eFilter.FilterValue.Any()) throw new InvalidOperationException($"Invalid Filter input parameter. {Enum.GetName(typeof(FilterType), filter.FilterType)} filter admit only 1 value.");

                    var comparatorType = typeof(DefaultComparator).Assembly.DefinedTypes.FirstOrDefault(x => x.Name == $"{Enum.GetName(typeof(LogialComparator), eFilter.FilterComparator)}") ?? typeof(DefaultComparator);
                    IComparator comparator = (IComparator)_serviceProvider.GetRequiredService(comparatorType);
                    filterBuilder = comparator.ComposeExpression(filterBuilder, eFilter, filter.TargetEntityId);
                }
            }

            return entity;

        }

        public async Task AddFilter(string targetEntity, List<ComplexFilter> complexFilter)
        {

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new InvalidDataException($"{nameof(targetEntity)} must have a value");
            }


            await Task.Run(() =>
            {
                var sessionId = _httpDecorator.GetSessionId() ?? throw new ArgumentNullException($"{nameof(_httpDecorator.GetSessionId)}");

                _cache.CreateAndSet(GetCacheKey(targetEntity.ToLower(), sessionId), complexFilter);
            });
        }

        public async Task DeleteFilter(string targetEntity)
        {

            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new InvalidDataException($"{nameof(targetEntity)} must have a value");
            }

            var sessionId = _httpDecorator.GetSessionId() ?? throw new ArgumentNullException($"{nameof(_httpDecorator.GetSessionId)}");

            await _cache.Remove(GetCacheKey(targetEntity.ToLower(), sessionId));

        }

        public async Task<Query> ApplyFilters<T>(Query entityQuery, string targetEntity, IEnumerable<ComplexFilter>? defaultFilters = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(targetEntity))
            {
                throw new InvalidDataException($"{nameof(targetEntity)} must have a value");
            }

            var cachedFilters = await GetFilters(targetEntity) ?? new List<ComplexFilter>();

            if (defaultFilters != null && defaultFilters.Any())
                cachedFilters.AddRange(defaultFilters);


            foreach (var filter in cachedFilters)
            {
                var serviceDefinedCorrelatorType = typeof(T).Assembly.DefinedTypes.FirstOrDefault(x => x.Name == filter.CorrelatorName)
                    ?? throw new InvalidOperationException($"Cannot find any {nameof(ICorrelator)} for name {filter}");

                ICorrelator correlatorIstance = (ICorrelator)_serviceProvider.GetRequiredService(serviceDefinedCorrelatorType);

                entityQuery = correlatorIstance.Correlate(entityQuery, filter);
            }

            entityQuery = ComposeWhereExpression(entityQuery, cachedFilters);

            return entityQuery;
        }
    }
}
