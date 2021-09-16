using System;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Synnotech.EntityFrameworkCore.MsSqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkCoreForMsSqlServer<TDbContext>(this IServiceCollection services,
                                                                                          string configurationSectionName = EfCoreSettings.DefaultSectionName,
                                                                                          ServiceLifetime dbContextLifetime = ServiceLifetime.Transient,
                                                                                          Action<SqlServerDbContextOptionsBuilder>? configureSqlServer = null,
                                                                                          Action<DbContextOptionsBuilder<TDbContext>>? configureOptions = null)
            where TDbContext : DbContext
        {
            services.MustNotBeNull(nameof(services));

            services.AddSingleton(container => EfCoreSettings.FromConfiguration(container.GetRequiredService<IConfiguration>(), configurationSectionName))
                    .AddSingleton(container =>
                     {
                         var settings = container.GetRequiredService<EfCoreSettings>();
                         var builder = new DbContextOptionsBuilder<TDbContext>().UseSqlServer(settings.ConnectionString, configureSqlServer)
                                                                                .EnableLoggingIfNecessary(container, settings.LoggingBehavior);
                         configureOptions?.Invoke(builder);
                         return builder.Options;
                     })
                    .AddDbContext<TDbContext>(dbContextLifetime);

            return services;
        }
    }
}