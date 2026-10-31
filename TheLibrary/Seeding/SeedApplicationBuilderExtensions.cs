using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TheLibrary.Seeding
{
    public static class SeedApplicationBuilderExtensions
    {
        public static async Task<WebApplication> UseInitialDataSeedAsync(this WebApplication app)
        {
            var seedEnabled = app.Configuration.GetValue<bool>("SeedData:Enabled");
            if (!seedEnabled)
            {
                return app;
            }

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("InitialDataSeed");

            await InitialDataSeeder.SeedAsync(services, app.Configuration, logger);

            return app;
        }
    }
}
