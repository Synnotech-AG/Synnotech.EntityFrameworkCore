using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synnotech.DatabaseAbstractions;

namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// <para>
    /// Represents an asynchronous session via an Entity Framework Core <see cref="DbContext" />. This session
    /// is able to start and commit several transactions individually by calling <see cref="BeginTransactionAsync" />.
    /// </para>
    /// <para>
    /// Beware: you must not derive from this class and introduce other references to disposable objects.
    /// Only the Context will be disposed.
    /// </para>
    /// </summary>
    /// <typeparam name="TDbContext">Your subtype that derives from <see cref="DbContext" />.</typeparam>
    public abstract class AsyncTransactionalSession<TDbContext> : AsyncSession<TDbContext>, IAsyncTransactionalSession
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncTransactionalSession{TDbContext}" />.
        /// </summary>
        /// <param name="context">The EF DbContext used for database access.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context" /> is null.</exception>
        protected AsyncTransactionalSession(TDbContext context) : base(context) { }

        /// <summary>
        /// <para>
        /// Begins a new transaction asynchronously. You must dispose the returned transaction by yourself. The
        /// session will not track any of the transactions that are created via this method.
        /// </para>
        /// <para>
        /// Be aware that commiting the transaction will not automatically call SaveChangesAsync! You must call SaveChangesAsync
        /// by yourself, otherwise committing will have no impact (this is how EF Core is implemented internally).
        /// </para>
        /// <para>
        /// Furthermore, you should ensure that a previous transaction has been committed before
        /// calling this method again - Synnotech.DatabaseAbstractions does not allow nested transactions.
        /// </para>
        /// </summary>
        public async Task<IAsyncTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
            return new AsyncEfTransaction(transaction);
        }
    }
}