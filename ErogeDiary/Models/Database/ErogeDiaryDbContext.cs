using ErogeDiary.Models.Database.Converters;
using ErogeDiary.Models.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Root> Roots { get; set; }
    public DbSet<PlayLog> PlayLogs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db")
            .UseLazyLoadingProxies()
            // ラムダ構文を使う必要がある
            // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging#logging-to-the-debug-window
            .LogTo(message => Debug.WriteLine(message))
            .EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Root>()
            .Property(r => r.Color)
            .HasConversion<ColorToStringConverter>();
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetUpdatedAtAndCreatedAtIfNeeded();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetUpdatedAtAndCreatedAtIfNeeded()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is BaseEntity
                && (e.State == EntityState.Added || e.State == EntityState.Modified));

        var now = DateTime.Now;
        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = now;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = now;
            }
        }
    }
}
