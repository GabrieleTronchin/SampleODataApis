using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Classic.Odata.DataModel
{
    internal class EntitiesHostedServices : IHostedService
    {
        private readonly ILogger<EntitiesHostedServices> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EntitiesHostedServices(ILogger<EntitiesHostedServices> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            var configurationContext = scope.ServiceProvider.GetRequiredService<WeatherContext>();

            if (configurationContext.Database.IsSqlServer())
            {
                _logger.LogInformation("Ensuring database exists using connection string {connectionString}", configurationContext.Database.GetDbConnection().ConnectionString);
                await configurationContext.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database ready");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}