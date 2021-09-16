using System;
using System.Threading;
using System.Threading.Tasks;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore.Storage;
using Synnotech.DatabaseAbstractions;

namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// Represents an adapter for the Entity Framework Core <see cref="IDbContextTransaction" /> that
    /// implements <see cref="IAsyncTransaction" />. The transaction will be implicitly rolled back
    /// when commit was not called and the transaction was disposed.
    /// </summary>
    public sealed class AsyncEfTransaction : IAsyncTransaction
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncEfTransaction" />.
        /// </summary>
        /// <param name="transaction">The EF transaction that will be wrapped by this instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="transaction" /> is null.</exception>
        public AsyncEfTransaction(IDbContextTransaction transaction) =>
            Transaction = transaction.MustNotBeNull(nameof(transaction));

        private IDbContextTransaction Transaction { get; }

        /// <summary>
        /// Disposes the underlying transaction. It will also be rolled back if <see cref="CommitAsync" />
        /// was not called.
        /// </summary>
        public ValueTask DisposeAsync() => Transaction.DisposeAsync();

        /// <summary>
        /// Disposes the underlying transaction. It will also be rolled back if <see cref="CommitAsync" />
        /// was not called.
        /// </summary>
        public void Dispose() => Transaction.Dispose();

        /// <summary>
        /// Commits the underlying EF transaction.
        /// </summary>
        public Task CommitAsync(CancellationToken cancellationToken = default) => Transaction.CommitAsync(cancellationToken);
    }
}