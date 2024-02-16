using Kata.Odata.DataModel.KataQuery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kata.Odata.DataModel
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddOdataKataQueryBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IODataQueryParser, ODataQueryParser>();
            services.AddTransient<IODataQueryBuilder, ODataQueryBuilder>();
            services.AddTransient<IEdmModelBuilder, EdmModelBuilder>();
            return services;
        }

    }
}