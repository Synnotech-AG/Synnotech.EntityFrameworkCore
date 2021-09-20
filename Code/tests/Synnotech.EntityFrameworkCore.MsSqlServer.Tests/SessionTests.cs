using System.Threading.Tasks;
using FluentAssertions;
using Light.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Synnotech.DatabaseAbstractions;
using Xunit;

namespace Synnotech.EntityFrameworkCore.MsSqlServer.Tests
{
    public static class SessionTests
    {
        [SkippableFact]
        public static async Task InsertAndUpdateData()
        {
            await TestSetup.SetupDatabaseOrSkipTestAsync();
            await using var container = TestSetup.PrepareContainer()
                                                 .AddSession<IChangeContactsSession, EfChangeContactsSession>()
                                                 .AddSession<IGetContactsSession, EfGetContactsSession>()
                                                 .BuildServiceProvider();

            await using (var changeSession = container.GetRequiredService<IChangeContactsSession>())
            {
                var john = await changeSession.GetContactByIdAsync(1);
                john = john.MustNotBeNull(nameof(john));
                john.Name = "John Johnson";

                var child = new Contact { Name = "Emil Johnson" };
                changeSession.AddContact(child);
                await changeSession.SaveChangesAsync();
            }

            await using var readSession = container.GetRequiredService<IGetContactsSession>();
            var loadedContacts = await readSession.GetContactsAsync();
            var expectedContacts = new Contact[]
            {
                new () { Id = 1, Name = "John Johnson" },
                new () { Id = 2, Name = "Margaret Johnson" },
                new () { Id = 3, Name = "Emil Johnson" }
            };
            loadedContacts.Should().Equal(expectedContacts);
        }
    }

    public interface IChangeContactsSession : IAsyncSession
    {
        ValueTask<Contact?> GetContactByIdAsync(int id);
        void AddContact(Contact contact);
    }

    public sealed class EfChangeContactsSession : AsyncSession<DatabaseContext>, IChangeContactsSession
    {
        public EfChangeContactsSession(DatabaseContext context) : base(context) { }

        public ValueTask<Contact?> GetContactByIdAsync(int id) => Context.Set<Contact?>().FindAsync(id);

        public void AddContact(Contact contact) => Context.Contacts.Add(contact);
    }
}