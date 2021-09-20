using Microsoft.EntityFrameworkCore;

namespace Synnotech.EntityFrameworkCore.MsSqlServer.Tests
{
    public record Contact
    {
        public int Id { get; init; }

        public string Name { get; set; } = string.Empty;
    }

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public virtual DbSet<Contact> Contacts => Set<Contact>();
    }
}