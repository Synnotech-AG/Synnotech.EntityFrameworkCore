using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Synnotech.EntityFrameworkCore.MsSqlServer.Tests
{
    public static class ReadOnlySessionTests
    {
        [SkippableFact]
        public static async Task LoadDataViaSession()
        {
            await TestSetup.SetupDatabaseOrSkipTestAsync();
            var container = TestSetup.PrepareContainer()
                                     .AddSession<IGetContactsSession, EfGetContactsSession>()
                                     .BuildServiceProvider();
            await using var session = container.GetRequiredService<IGetContactsSession>();
            var loadedContacts = await session.GetContactsAsync();

            var expectedContacts = new Contact[]
            {
                new () { Id = 1, Name = "John Doe" },
                new () { Id = 2, Name = "Margaret Johnson"}
            };
            loadedContacts.Should().Equal(expectedContacts);
        }
    }
}