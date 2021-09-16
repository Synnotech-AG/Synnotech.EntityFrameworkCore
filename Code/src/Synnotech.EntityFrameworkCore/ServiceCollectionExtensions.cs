using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Synnotech.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection services, ServiceLifetime dbContextLifetime = ServiceLifetime.Transient)
            where TDbContext : DbContext
        {
            services.MustNotBeNull(nameof(services));
            services.Add(new ServiceDescriptor(typeof(TDbContext), typeof(TDbContext), dbContextLifetime));
            return services;
        }

        public static DbContextOptionsBuilder<TDbContext> EnableLoggingIfNecessary<TDbContext>(this DbContextOptionsBuilder<TDbContext> builder, IServiceProvider container, LoggingBehavior behavior)
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