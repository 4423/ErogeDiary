using System;

namespace ErogeDiary.Models.Database.Entities;

// カラム定義
public partial class PlayLog : BaseEntity
{
    public int PlayLogId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; }
}

// DB には保存しない値
public partial class PlayLog : BaseEntity
{
    public TimeSpan PlayTime => EndedAt - StartedAt;
}