using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<RootData> Roots { get; set; }
    public DbSet<PlayLog> PlayLogs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db")
            // ラムダ構文を使う必要がある
            // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging#logging-to-the-debug-window
            .LogTo(message => Debug.WriteLine(message))
            .EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Roots)
            .WithOne();

        base.OnModelCreating(modelBuilder);
    }
}
