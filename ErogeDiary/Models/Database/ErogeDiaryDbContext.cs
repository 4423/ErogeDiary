using Microsoft.EntityFrameworkCore;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<RootData> Roots { get; set; }
    public DbSet<PlayLog> PlayLogs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
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
