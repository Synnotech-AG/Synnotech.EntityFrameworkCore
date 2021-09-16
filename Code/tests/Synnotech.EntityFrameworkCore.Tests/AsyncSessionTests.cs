using System.Threading.Tasks;
using FluentAssertions;
using Synnotech.DatabaseAbstractions;
using Xunit;

namespace Synnotech.EntityFrameworkCore.Tests
{
    public static class AsyncSessionTests
    {
        [Fact]
        public static void MustImplementIAsyncSession() =>
            typeof(AsyncSession<>).Should().Implement<IAsyncSession>();

        [Fact]
        public static void MustDeriveFromAsyncReadOnlySession() =>
            typeof(AsyncSession<>).Should().BeDerivedFrom(typeof(AsyncReadOnlySession<>));

        [Fact]
        public static async Task InsertNewContact()
        {
            var context = TestContext.CreateInMemory();
            var contact = new Contact { Name = "Foo" };

            await using (var session = new EfAddContactSession(context))
            {
                session.AddContact(contact);
                await session.SaveChangesAsync();
            }

            contact.Id.Should().Be(1);
            context.SaveChangesMustHaveBeenCalled()
                   .MustHaveBeenDisposed();
        }

        private interface IAddContactSession : IAsyncSession
        {
            void AddContact(Contact contact);
        }

        private sealed class EfAddContactSession : AsyncSession<TestContext>, IAddContactSession
        {
            public EfAddContactSession(TestContext context) : base(context) { }

            public void AddContact(Contact contact) => Context.Contacts.Add(contact);
        }
    }
}