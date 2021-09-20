using System.Threading.Tasks;
using Light.EmbeddedResources;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synnotech.MsSqlServer;
using Synnotech.Xunit;
using Xunit;

namespace Synnotech.EntityFrameworkCore.MsSqlServer.Tests
{
    public static class TestSetup
    {
        public static async Task<string> SetupDatabaseOrSkipTestAsync()
        {
            Skip.IfNot(TestSettings.Configuration.GetValue<bool>("database:areIntegrationTestsEnabled"));

            var connectionString = TestSettings.Configuration["database:connectionString"];
            if (connectionString.IsNullOrWhiteSpace())
                throw new InvalidConfigurationException("You must provide a valid connection string when \"areIntegrationTestsEnabled\" is set to true");

            await Database.DropAndCreateDatabaseAsync(connectionString);
            await Database.ExecuteNonQueryAsync(connectionString, typeof(TestSetup).GetEmbeddedResource("ExampleDatabase.sql"));
            return connectionString;
        }

        public static IServiceCollection PrepareContainer() =>
            new ServiceCollection().AddSingleton(TestSettings.Configuration)
                                   .AddEntityFrameworkCoreForMsSqlServer<DatabaseContext>();
    }
}