using Kata.QueryBuilder.ComplexFilters.FilterComparators;
using Kata.QueryBuilder.ComplexFilters.FilterTypeParsers;
using Kata.QueryBuilder.ComplexFilters.Mapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceCache;

namespace Kata.QueryBuilder.ComplexFilters
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddComplexFilterMapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFilterComparators();
            services.AddFilterParser();
            services.AddServiceCache(configuration);
            services.AddTransient<IKataComplexFilterManager, KataComplexFilterManager>();
            services.AddOptions<CacheOptions>().Bind(configuration.GetSection(nameof(KataComplexFilterManager))).ValidateDataAnnotations();
            services.AddSingleton<IComplexFilterMapper<ComplexFilterMapper>, ComplexFilterMapper>();
            return services;
        }


        private static IServiceCollection AddFilterComparators(this IServiceCollection services)
        {
            services.AddTransient<ContainsComparator>();
            services.AddTransient<DefaultComparator>();
            services.AddTransient<GreaterThanComparator>();
            services.AddTransient<GreaterThanOrEqualComparator>();
            services.AddTransient<LessThanComparator>();
            services.AddTransient<LessThanOrEqualComparator>();
            services.AddTransient<StartWithComparator>();
            services.AddTransient<EndWithComparator>();
            services.AddTransient<NotEqualsComparators>();
            services.AddTransient<EqualsComparator>();
            services.AddTransient<IsNullComparator>();
            services.AddTransient<IsNotNullComparator>();
            return services;
        }


        private static IServiceCollection AddFilterParser(this IServiceCollection services)
        {
            services.AddTransient<DefaultFilterTypeParser>();
            services.AddTransient<RangeSelectionTypeParser>();
            services.AddTransient<SingleSelectionTypeParser>();
            return services;
        }




    }
}
