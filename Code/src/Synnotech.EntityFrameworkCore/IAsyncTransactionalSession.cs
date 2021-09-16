using Synnotech.DatabaseAbstractions;

namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// Represents an asynchronous session to a database that is able to create individual transactions.
    /// Use this interface if you are using EF core in the background instead of the one from <see cref="DatabaseAbstractions.IAsyncTransactionalSession" />,
    /// because you need an additional SaveChangesAsync method.
    /// </summary>
    public interface IAsyncTransactionalSession : DatabaseAbstractions.IAsyncTransactionalSession, IAsyncSession { }
}