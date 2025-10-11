namespace ConsoleApp1.Data;

using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Data;
public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SampleRecord>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<SampleRecord>()
            .Property(e => e.CreatedAt)
            .IsRequired();

        modelBuilder.Entity<FooEntity>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
    }

    public DbSet<SampleRecord> SampleData { get; set; } = null!;
    public DbSet<FooEntity> FooEntities { get; set; } = null!;
}
