using System;
using System.Threading.Tasks;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Synnotech.DatabaseAbstractions;

namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// <para>
    /// Represents an asynchronous database session via an Entity Framework Core <see cref="DbContext" />. This
    /// session is only used to read data (i.e. no data is inserted, updated, or deleted), thus SaveChangesAsync
    /// is not available. No transaction is needed while this session is active.
    /// </para>
    /// <para>
    /// Beware: you must not derive from this class and introduce other references to disposable objects.
    /// Only the <see cref="Context" /> will be disposed.
    /// </para>
    /// </summary>
    /// <typeparam name="TDbContext">Your subtype that derives from <see cref="DbContext" />.</typeparam>
    public abstract class AsyncReadOnlySession<TDbContext> : IAsyncReadOnlySession
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncReadOnlySession{TDbContext}" />.
        /// </summary>
        /// <param name="context">The EF DbContext used for database access.</param>
        /// <param name="disableQueryTracking">
        /// The value indicating whether the query tracking behavior will be set to <see cref="QueryTrackingBehavior.NoTracking" /> (optional).
        /// The default value is true. Read-only sessions usually do not need change tracking as they will not call SaveChangesAsync.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context" /> is null.</exception>
        protected AsyncReadOnlySession(TDbContext context, bool disableQueryTracking = true)
        {
            Context = context.MustNotBeNull(nameof(context));
            if (disableQueryTracking)
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Gets the DB context of Entity Framework Core.
        /// </summary>
        protected TDbContext Context { get; }

        /// <summary>
        /// Disposes the DB context.
        /// </summary>
        public void Dispose() => Context.Dispose();

        /// <summary>
        /// Disposes the DB context.
        /// </summary>
        public ValueTask DisposeAsync() => Context.DisposeAsync();
    }
}