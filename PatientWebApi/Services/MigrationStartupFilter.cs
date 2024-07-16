using Microsoft.EntityFrameworkCore;
using PatientWebApi.Context;
using System;

namespace PatientWebApi.Services
{
    public class MigrationStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                next(app);

                using var scope = app.ApplicationServices.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                try
                {
                    var dbContext = serviceProvider.GetRequiredService<PatientDbContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<PatientDbContext>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            };
        }
    }
}
