using System;

namespace ErogeDiary.Models.Database.Entities;

public class PlayLog : BaseEntity
{
    public int PlayLogId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public TimeSpan PlayTime { get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; }
}
