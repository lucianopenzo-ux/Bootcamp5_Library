using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TheLibrary.Seeding
{
    public static class SeedApplicationBuilderExtensions
    {
        /// <summary>
        /// Ejecuta la semilla de datos inicial si está habilitada en appsettings.json
        /// </summary>
        /// <param name="app">La aplicación web</param>
        /// <returns>La aplicación web con la semilla de datos ejecutada si está habilitada</returns>

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
