using MaraudersMap.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MaraudersMap.Data.DbContexts
{
    public class MarauderDbContext: DbContext
    {
        public MarauderDbContext(DbContextOptions<MarauderDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MarauderDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        public DbSet<MarauderUser> MarauderUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MarauderUser>()
                .Property(c => c.Name)
                .HasMaxLength(30);

            modelBuilder.Entity<MarauderUser>()
                .Property(c => c.Coordinates)
                .HasMaxLength(30);
        }
    }
}
