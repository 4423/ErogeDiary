using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ErogeDiary.Models.Database.Entities;

// カラム定義
public partial class Game : BaseEntity
{
    public int GameId { get; set; }
    public required string Title { get; set; }
    public required string Brand { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public required string ImageFileName { get; set; } // 画像のファイル名のみ
    public string? ErogameScapeGameId { get; set; }
    public InstallationType InstallationType { get; set; }
    public string? ExecutableFilePath { get; set; } // InstallationType = Default のとき required
    public string? WindowTitle { get; set; } // InstallationType = DmmGamePlayer のとき required
    public DateTime RegisteredAt { get; set; }
    public DateTime? LastPlayedAt { get; set; }
    public DateTime? ClearedAt { get; set; }
    public TimeSpan TotalPlayTime { get; set; } // 毎回 PlayLog を集計するのは大変なので

    public virtual ICollection<Root> Roots { get; private set; } 
        = new ObservableCollection<Root>();
    public virtual ICollection<PlayLog> PlayLogs { get; private set; } 
        = new ObservableCollection<PlayLog>();
}

// DB には保存しない値
public partial class Game
{
    public bool IsCleared => ClearedAt != null;

    public bool CanLaunch => InstallationType == InstallationType.Default;

    public Uri ImageUri => new Uri(Path.Combine(ThumbnailHelper.ThumbnailDir, ImageFileName));

    public TimeSpan GetUnallocatedTime()
    {
        if (Roots == null || Roots.Count == 0)
        {
            return TotalPlayTime;
        }
        var sumOfRootMilliseconds = Roots.Sum(r => r.PlayTime.TotalMilliseconds);
        var unallocatedMilliseconds = Math.Max(0, TotalPlayTime.TotalMilliseconds - sumOfRootMilliseconds);
        return TimeSpan.FromMilliseconds(unallocatedMilliseconds);
    }
}

public enum InstallationType
{
    Default,
    DmmGamePlayer,
}