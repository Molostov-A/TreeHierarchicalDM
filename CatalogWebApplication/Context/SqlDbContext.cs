using CatalogWebApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogWebApplication.Context
{
    public class SqlDbContext : DbContext
    {
        public DbSet<Catalog> Categories { get; set; }

        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Catalog>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Catalog>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Catalog>()
                .HasIndex(c => c.ParentId);
        }
    }
}
