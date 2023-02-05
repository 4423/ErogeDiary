using ErogeDiary.Models.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ErogeDiary.Models.Database;

public partial class ErogeDiaryDbContext
{
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

    public IEnumerable<PlayLog> FindPlayLogsByGameIdAndDateRange(int gameId, DateOnly startInclusive, DateOnly endInclusive)
    {
        var startInclusiveDateTime = startInclusive.ToDateTime(TimeOnly.MinValue);
        var endInclusiveDateTime = endInclusive.ToDateTime(TimeOnly.MaxValue);

        return PlayLogs.Where(p => 
            p.GameId == gameId
            && startInclusiveDateTime <= p.StartedAt 
            && p.StartedAt <= endInclusiveDateTime
        );
    }

    public Task<PlayLog?> FindFirstPlayLogByGameId(int gameId)
    {
        return PlayLogs.Where(p => p.GameId == gameId).OrderBy(p => p.StartedAt).FirstOrDefaultAsync();
    }

    public IEnumerable<int> GetPlayLogYears(int gameId)
    {
        return PlayLogs.Where(p => p.GameId == gameId).Select(p => p.StartedAt.Year).Distinct();
    }
}
