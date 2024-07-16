namespace PatientWebApi.Services
{
    public static class InfrastructureServiceInitialisation
    {
        public static IServiceCollection AddContextMigration(this IServiceCollection services)
        {
            services.AddTransient<IStartupFilter, MigrationStartupFilter>();
            return services;
        }
    }
}
