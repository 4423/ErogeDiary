using ErogeDiary.Models.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext
{
    public async Task AddGameAsync(Game game)
    {
        Games.Add(game);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        Games.Update(game);
        await SaveChangesAsync();
    }

    public async Task RemoveAsync(Game game)
    {
        Games.Remove(game);
        await SaveChangesAsync();
    }

    public async Task<ObservableCollection<Game>> GetGamesAsync()
    {
        await Games.LoadAsync();
        await Roots.LoadAsync();
        return Games.Local.ToObservableCollection();
    }

    public async Task<Game?> FindGameByFileNameAsync(string fileName)
    {
        return await Games.SingleOrDefaultAsync(x => x.ExecutableFilePath == fileName);
    }

    public async Task<Game?> FindGameByWindowTitleAsync(string windowTitle)
    {
        // 複数件該当する可能性はあるが最初の1件を返しておく 
        return await Games.Where(x => x.WindowTitle != null)
            .FirstOrDefaultAsync(x => windowTitle.Contains(x.WindowTitle!));
    }

    public async Task<Game?> FindGameByTitleAndBrandAsync(string title, string brand)
    {
        return await Games.SingleOrDefaultAsync(x => x.Title == title && x.Brand == brand);
    }
}
