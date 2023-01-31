using System;
using System.Windows.Media;

namespace ErogeDiary.Models.Database.Entities;

// カラム定義
public partial class Root : BaseEntity
{
    public int RootId { get; set; }
    public string Name { get; set; }
    public TimeSpan PlayTime { get; set; }
    public DateTime? ClearedAt { get; set; }
    public Color Color { get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; }
}

// DB には保存しない値
public partial class Root
{
    public bool IsCleared => ClearedAt != null;
}