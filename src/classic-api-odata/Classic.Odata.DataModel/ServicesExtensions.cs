using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Classic.Odata.DataModel
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddDataModel(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddDbContext<WeatherContext>(options, lifetime);
            services.TryAddSingleton<IHostedService, EntitiesHostedServices>();

            return services;
        }
    }
}