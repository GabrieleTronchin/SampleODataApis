using Kata.QueryBuilder.ComplexFilters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kata.QueryBuilder
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddOdataKataQueryBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddComplexFilterMapper(configuration);
            services.AddTransient<IODataQueryParser, ODataQueryParser>();
            services.AddTransient<IODataQueryBuilder, ODataQueryBuilder>();
            services.AddTransient<IEdmModelBuilder, EdmModelBuilder>();

            return services;
        }

    }
}