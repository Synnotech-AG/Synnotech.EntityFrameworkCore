using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Synnotech.DatabaseAbstractions;
using Xunit;

namespace Synnotech.EntityFrameworkCore.Tests
{
    public static class AsyncReadOnlySessionTests
    {
        [Fact]
        public static void MustImplementIAsyncReadOnlySession() =>
            typeof(AsyncReadOnlySession<>).Should().Implement<IAsyncReadOnlySession>();

        [Fact]
        public static async Task LoadData()
        {
            var contacts = new Contact[]
            {
                new () { Id = 1, Name = "John Doe" },
                new () { Id = 2, Name = "Margaret Jones" }
            };
            var context = TestContext.CreateInMemory();
            await context.Contacts.AddRangeAsync(contacts);
            await context.SaveChangesAsync();

            List<Contact> loadedContacts;
            await using (var session = new EfGetContactsSession(context))
            {
                loadedContacts = await session.GetContactsAsync();
            }

            loadedContacts.Should().BeEquivalentTo(contacts);
            context.MustHaveBeenDisposed();
        }

        private interface IGetContactsSession : IAsyncReadOnlySession
        {
            Task<List<Contact>> GetContactsAsync();
        }

        private sealed class EfGetContactsSession : AsyncReadOnlySession<TestContext>, IGetContactsSession
        {
            public EfGetContactsSession(TestContext context) : base(context) { }

            public Task<List<Contact>> GetContactsAsync() =>
                Context.Contacts.ToListAsync();
        }
    }
}