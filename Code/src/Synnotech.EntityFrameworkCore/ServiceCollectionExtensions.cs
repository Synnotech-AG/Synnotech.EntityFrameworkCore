using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// Provides extension methods for registering Entity Framework Core with the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers your DB context with the DI container.
        /// </summary>
        /// <typeparam name="TDbContext">Your subtype that derives from <see cref="DbContext" />.</typeparam>
        /// <param name="services">The collection that holds all registrations for the DI container.</param>
        /// <param name="dbContextLifetime">
        /// The lifetime of the DB context (optional). The default value is <see cref="ServiceLifetime.Transient" />.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
        public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection services,
                                                                  ServiceLifetime dbContextLifetime = ServiceLifetime.Transient)
            where TDbContext : DbContext
        {
            services.MustNotBeNull(nameof(services))
                    .Add(new ServiceDescriptor(typeof(TDbContext), typeof(TDbContext), dbContextLifetime));
            return services;
        }

        /// <summary>
        /// Enables Entity Framework Core logging based on the specified logging behavior.
        /// </summary>
        /// <typeparam name="TDbContext">Your subtype that derives from <see cref="DbContext" />.</typeparam>
        /// <param name="builder">The options builder for the DB context.</param>
        /// <param name="container">The DI container that is used to resolve an <see cref="ILoggerFactory" /> when logging is enabled.</param>
        /// <param name="behavior">The value indicating whether logging is enabled.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder" /> or <paramref name="container" /> is null.</exception>
        public static DbContextOptionsBuilder<TDbContext> EnableLoggingIfNecessary<TDbContext>(this DbContextOptionsBuilder<TDbContext> builder,
                                                                                               IServiceProvider container,
                                                                                               LoggingBehavior behavior)
            where TDbContext : DbContext
        {
            builder.MustNotBeNull(nameof(builder));
            container.MustNotBeNull(nameof(container));

            if (behavior == LoggingBehavior.Off)
                return builder;

            var loggerFactory = container.GetService<ILoggerFactory>();
            if (loggerFactory == null)
                throw new InvalidConfigurationException($"The logging behavior for EF Core is set to \"{behavior}\", but there is no ILoggerFactory registered with the DI container.");

            builder.UseLoggerFactory(loggerFactory);
            if (behavior == LoggingBehavior.EnabledWithSensitiveData)
                builder.EnableSensitiveDataLogging();

            return builder;
        }
    }
}