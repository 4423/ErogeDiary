using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary.Models.Database
{
    public class SQLiteDatabaseAccess : DbContext, IDatabaseAccess
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

        #region Games

        public async Task AddGameAsync(Game game)
        {
            Games.Add(game);
            await this.SaveChangesAsync();
        }

        public async Task UpdateAsync(Game game)
        {
            Games.Update(game);
            await this.SaveChangesAsync();
        }

        public async Task<Game> FindGameByFileNameAsync(string fileName)
        {
            return await Games.SingleOrDefaultAsync(x => x.FileName == fileName);            
        }

        public async Task<Game> FindGameByWindowTitleAsync(string windowTitle)
        {
            // 複数件該当する可能性はあるが最初の1件を返しておく 
            return await Games.Where(x => x.WindowTitle != null)
                .FirstOrDefaultAsync(x => windowTitle.Contains(x.WindowTitle!));
        }

        public async Task<Game> FindGameByTitleAndBrandAsync(string title, string brand)
        {
            return await Games.SingleOrDefaultAsync(x => x.Title == title && x.Brand == brand);
        }

        public async Task<ObservableCollection<Game>> GetGamesAsync()
        {
            await Games.LoadAsync();
            await Roots.LoadAsync();
            return Games.Local.ToObservableCollection();
        }

        public async Task RemoveAsync(Game game)
        {
            Games.Remove(game);
            await this.SaveChangesAsync();
        }

        #endregion

        #region Roots

        public async Task AddRootAsync(RootData root)
        {
            Roots.Add(root);
            await SaveChangesAsync();
        }

        public async Task UpdateRootAsync(RootData root)
        {
            Roots.Update(root);
            await SaveChangesAsync();
        }

        public async Task RemoveRootAsync(RootData root)
        {
            Roots.Remove(root);
            await SaveChangesAsync();
        }

        #endregion

        #region PlayLog

        public async Task<ObservableCollection<PlayLog>> GetPlayLogsAsync()
        {
            await PlayLogs.LoadAsync();
            return PlayLogs.Local.ToObservableCollection();
        }

        public async Task AddPlayLogAsync(PlayLog playLog)
        {
            PlayLogs.Add(playLog);
            await SaveChangesAsync();
        }

        public IEnumerable<PlayLog> FindPlayLogsByGameId(int gameId)
        {
            return PlayLogs.Where(p => p.GameId == gameId);
        }

        public IEnumerable<PlayLog> FindPlayLogsByGameId(int gameId, DateTime since)
        {
            return PlayLogs.Where(p => p.GameId == gameId && since <= p.StartDateTime);
        }

        #endregion
    }
}
