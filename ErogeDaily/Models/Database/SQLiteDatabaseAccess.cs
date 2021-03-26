using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models.Database
{
    public class SQLiteDatabaseAccess : DbContext, IDatabaseAccess
    {
        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
            base.OnConfiguring(optionsBuilder);
        }

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

        public async Task<Game> FindGameByTitleAndBrandAsync(string title, string brand)
        {
            return await Games.SingleOrDefaultAsync(x => x.Title == title && x.Brand == brand);
        }

        public async Task<ObservableCollection<Game>> GetGamesAsync()
        {
            await Games.LoadAsync();
            return Games.Local.ToObservableCollection();
        }
    }
}
