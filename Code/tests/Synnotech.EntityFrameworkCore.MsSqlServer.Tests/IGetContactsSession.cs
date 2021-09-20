using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synnotech.DatabaseAbstractions;

namespace Synnotech.EntityFrameworkCore.MsSqlServer.Tests
{
    public interface IGetContactsSession : IAsyncReadOnlySession
    {
        Task<List<Contact>> GetContactsAsync();
    }

    public sealed class EfGetContactsSession : AsyncReadOnlySession<DatabaseContext>, IGetContactsSession
    {
        public EfGetContactsSession(DatabaseContext context) : base(context) { }

        public Task<List<Contact>> GetContactsAsync() => Context.Contacts.ToListAsync();
    }
}