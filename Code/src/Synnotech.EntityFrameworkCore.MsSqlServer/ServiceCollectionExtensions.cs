using System;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Synnotech.EntityFrameworkCore.MsSqlServer
{
    /// <summary>
    /// Provides extensions methods to register Entity Framework Core for MS SQL server with the DI container
    /// and integrating with the <see cref="IConfiguration" /> infrastructure.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// <para>
        /// Registers Entity Framework Core with the DI container. This is done by loading <see cref="EfCoreSettings" />
        /// from the <see cref="IConfiguration" /> (that must already be registered with the container) and register
        /// these settings as a singleton. Also, DbContextOptions&lt;TDbContext&gt; is registered as a singleton and
        /// configured via the <see cref="EfCoreSettings" /> (connection string and logging). Finally, your <typeparamref name="TDbContext" />
        /// is registered with the DI container, by default using a transient lifetime.
        /// </para>
        /// <para>
        /// You can customize this registration process by providing a different <paramref name="dbContextLifetime" />, or by providing the
        /// <paramref name="configureSqlServer" /> and <paramref name="configureOptions" /> delegates where you can manually adjust the SQL server
        /// options and the DB context options.
        /// </para>
        /// </summary>
        /// <typeparam name="TDbContext">Your subtype that derives from <see cref="DbContext" />.</typeparam>
        /// <param name="services">The collection that holds all registrations for the DI container.</param>
        /// <param name="configurationSectionName">
        /// The name of the configuration section that will be deserialized as <see cref="EfCoreSettings" /> (optional).
        /// The default value is "database".
        /// </param>
        /// <param name="dbContextLifetime">
        /// The lifetime the <typeparamref name="TDbContext" /> is registered with the DI container (optional).
        /// The default value is transient. In general, we recommend that an orchestrator (such as a controller) is
        /// responsible for opening and closing a database session, instead of relying on the DI container doing that
        /// implicitly. If you want to stray from this principle, you can provide <see cref="ServiceLifetime.Scoped" />.
        /// </param>
        /// <param name="configureSqlServer">The delegate that further configures the SQL server access (optional).</param>
        /// <param name="configureOptions">The delegate that further configures the DB context options.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
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