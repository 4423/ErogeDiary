using System;
using System.ComponentModel;

namespace ErogeDiary.Models.Database.Entities;

// AssemblyInfo.cs での指定により、この namespace にのみ Fody.PropertyChanged が適用されている
public abstract class BaseEntity : INotifyPropertyChanged
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}
