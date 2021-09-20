﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synnotech.DatabaseAbstractions;

namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// <para>
    /// Represents an asynchronous session via an Entity Framework Core <see cref="DbContext" />. This
    /// type of session can be used to read as well as insert, update, or delete data. The session automatically
    /// uses a transaction when calling <see cref="SaveChangesAsync" /> (as implemented in EF Core). If you
    /// do not call <see cref="SaveChangesAsync" />, your changes will automatically discarded.
    /// </para>
    /// <para>
    /// Beware: you must not derive from this class and introduce other references to disposable objects.
    /// Only the Context will be disposed.
    /// </para>
    /// </summary>
    /// <typeparam name="TDbContext">Your subtype that derives from <see cref="DbContext" />.</typeparam>
    public abstract class AsyncSession<TDbContext> : AsyncReadOnlySession<TDbContext>, IAsyncSession
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncSession{TDbContext}" />.
        /// </summary>
        /// <param name="context">The EF DbContext used for database access.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context" /> is null.</exception>
        protected AsyncSession(TDbContext context) : base(context, false) { }

        /// <summary>
        /// Calls SaveChangesAsync on the internal DB context of Entity Framework Core.
        /// </summary>
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// <para>
    /// Represents an asynchronous session via an Entity Framework Core <see cref="DbContext" />. This
    /// type of session can be used to read as well as insert, update, or delete data. The session automatically
    /// uses a transaction when calling SaveChangesAsync (as implemented in EF Core). If you
    /// do not call SaveChangesAsync, your changes will automatically discarded.
    /// </para>
    /// <para>
    /// Beware: you must not derive from this class and introduce other references to disposable objects.
    /// Only the Context will be disposed.
    /// </para>
    /// </summary>
    public abstract class AsyncSession : AsyncSession<DbContext>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncSession" />.
        /// </summary>
        /// <param name="context">The EF DbContext used for database access.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context" /> is null.</exception>
        protected AsyncSession(DbContext context) : base(context) { }
    }
}