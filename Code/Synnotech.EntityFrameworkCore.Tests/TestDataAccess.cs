using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;

namespace Synnotech.EntityFrameworkCore.Tests
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options) : base(options) { }

        public DbSet<Contact> Contacts => Set<Contact>();

        public int DisposeCallCount { get; private set; }

        public int SaveChangesCallCount { get; private set; }

        public static TestContext CreateInMemory([CallerMemberName] string? databaseName = null)
        {
            databaseName.MustNotBeNull(nameof(databaseName));

            var options = new DbContextOptionsBuilder<TestContext>().UseInMemoryDatabase(databaseName!)
                                                                    .Options;
            return new TestContext(options);
        }

        public override void Dispose()
        {
            DisposeCallCount++;
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            DisposeCallCount++;
            return base.DisposeAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SaveChangesCallCount++;
            return base.SaveChanges();
        }

        public TestContext SaveChangesMustHaveBeenCalled()
        {
            SaveChangesCallCount.Should().Be(1);
            return this;
        }

        public TestContext MustHaveBeenDisposed()
        {
            DisposeCallCount.Should().BeGreaterThan(0);
            return this;
        }
    }

    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}