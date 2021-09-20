# Synnotech.EntityFrameworkCore.MsSqlServer

*Provides composition root extensions for Synnotech.EntityFrameworkCore used in combination with MS SQL Server.*

[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/Synnotech-AG/Synnotech.EntityFrameworkCore/blob/main/LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg?style=for-the-badge)](https://www.nuget.org/packages/Synnotech.EntityFrameworkCore.MsSqlServer/)

# How to install

Synnotech.EntityFrameworkCore.MsSqlServer is compiled against [.NET Standard 2.0 and 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) and thus supports all major plattforms like .NET 5, .NET Core, .NET Framework 4.6.1 or newer, Mono, Xamarin, UWP, or Unity.

Synnotech.EntityFrameworkCore.MsSqlServer is available as a [NuGet package](https://www.nuget.org/packages/Synnotech.EntityFrameworkCore.MsSqlServer/) and can be installed via:

- **Package Reference in csproj**: `<PackageReference Include="Synnotech.EntityFrameworkCore.MsSqlServer" Version="1.0.0" />`
- **dotnet CLI**: `dotnet add package Synnotech.EntityFrameworkCore.MsSqlServer`
- **Visual Studio Package Manager Console**: `Install-Package Synnotech.EntityFrameworkCore.MsSqlServer`

# What does Synnotech.EntityFrameworkCore.MsSqlServer offer you?

Synnotech.EntityFrameworkCore implements the session abstractions of [Synnotech.DatabaseAbstractions](https://github.com/Synnotech-AG/Synnotech.DatabaseAbstractions) for Entity Framework Core 5 or newer (or 3.1 for .NET Standard 2.0-compatible platforms). This allows you to simplify the code in your data access layer. Furthermore, Synnotech.EntityFrameworkCore.MsSqlServer allows you to configure your DI container with one call when you want to use the Synnotech default settings.

Please see the [general docs](https://github.com/Synnotech-AG/Synnotech.EntityFrameworkCore/blob/main/readme.md) on how to write custom session.

# Default configuration

Synnotech.EntityFrameworkCore.MsSqlServer provides an extension method for `IServiceCollection` to easily get you started in e.g. ASP.NET Core Apps. Simply call `AddEntityFrameworkCoreForMsSqlServer`:

```csharp
public void ConfigureService(IServiceCollection services)
{
    // All other services like MVC are left out for brevity's sake 
    services.AddEntityFrameworkCoreForMsSqlServer<DatabaseContext>();
}
```

Your custom DB context class should look like the following:

```csharp
public class DatabaseContext : DbContext
{
    // Your database context should have a single constructor that takes options
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base (options) { }

    public DbSet<Contact> Contacts => Set<Contact>();

    // other members omitted for brevity's sake
}
```

The method will do the following:

1. Load the `EfCoreSettings` from the `IConfiguration` instance that is already present in the DI container and register it as a singleton.
1. Use the `EfCoreSettings` to configure a `DbContextOptionsBuilder<T>` instance and register it as a singleton. On the corresponding options builder, `UseSqlServer` is called and logging is enabled if specified in the `EfCoreSettings`.
1. Register your `DatabaseContext` with a transient lifetime.

You can configure your EF Core DB context via your appsettings.json file

```jsonc
{
    // other configuration sections are left out for brevity's sake
    "database": {
        "connectionString": "Server=(localdb)\\MsSqlLocalDB;Database=MyDatabase;Integrated Security=True",
        "loggingBehavior": "Off" // Corresponds to Synnotech.EntityFrameworkCore.LoggingBehavior, used for EF Core internal logging 
    }
}
```

When you set `loggingBehavior` to a value other than `LoggingBehavior.Off`, your services also need to have an `ILoggerFactory` (from Microsoft.Extensions.Logging) registered. It will be used for DB context tracing. We recommend to leave it off because it has a severe impact on performance and log file size - only enable logging when you need to see the SQL statements created by EF Core.

Please note: the configuration is only loaded once during startup. If you change the database settings, you need to restart your web app.

`AddEntityFrameworkCoreForMsSqlServer` has several optional parameters that you can supply:

- `configurationSectionName`: the section name that is used to retrieve the `EfCoreSettings` from the `IConfiguration` instance already registered with the DI container. The default value is "database".
- `dbContextLifetime`: the enumeration value indicating the service lifetime for your `DatabaseContext`. The default value is `ServiceLifetime.Transient`.
- `configureSqlServer`: an optional delegate that takes a `SqlServerDbContextOptionsBuilder` to further configure the access to MS SQL Server.
- `configureOption`: an optional delegate that takes a `DbContextOptionsBuilder<DatabaseContext>` where you can further configure the DB context options. This delegate will be called after `UseSqlServer` and `EnableLoggingIfNecessary` is called.

If you don't want to use `AddEntityFrameworkCoreForMsSqlServer`, you might still want to reuse its internal functionality. Just take a look at the source code of `EfCoreSettings` and `EnableLoggingIfNecessary`.