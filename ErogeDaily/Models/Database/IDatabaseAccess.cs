using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models.Database
{
    public interface IDatabaseAccess
    {
        public Task AddGameAsync(Game game);
        public Task UpdateAsync(Game game);
        public Task<ObservableCollection<Game>> GetGamesAsync();
        public Task<Game> FindGameByFileNameAsync(string fileName);
        public Task<Game> FindGameByTitleAndBrandAsync(string title, string brand);
        public Task RemoveAsync(Game game);

        public Task<ObservableCollection<PlayLog>> GetPlayLogsAsync();
        public Task AddPlayLogAsync(PlayLog playLog);
        public IEnumerable<PlayLog> FindPlayLogsByGameId(int gameId);
    }
}
