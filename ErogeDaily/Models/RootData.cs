using ErogeDiary.Controls;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public class RootData : VerifiableBindableBase, ICloneable
    {
        private int id;
        public int Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        private string name;
        [Required(ErrorMessage = "ルート名を入力してください。")]
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
                ValidateProperty(value);
            }
        }

        private TimeSpan playTime;
        [Required(ErrorMessage = "プレイ時間を入力してください。")]
        public TimeSpan PlayTime
        {
            get => playTime;
            set { SetProperty(ref playTime, value); }
        }

        private bool isCleared;
        public bool IsCleared
        {
            get => isCleared;
            set { SetProperty(ref isCleared, value); }
        }

        private DateTime? clearedAt;
        public DateTime? ClearedAt
        {
            get => clearedAt;
            set { SetProperty(ref clearedAt, value); }
        }

        private DateTime createdAt;
        public DateTime CreatedAt
        {
            get => createdAt;
            set { SetProperty(ref createdAt, value); }
        }

        private DateTime updatedAt;
        public DateTime UpdatedAt
        {
            get => updatedAt;
            set { SetProperty(ref updatedAt, value); }
        }


        public object Clone()
        {
            return MemberwiseClone();
        }

        public void CopyFrom(RootData other)
        {
            if (Id != other.Id)
            {
                throw new ArgumentException();
            }
            Name = other.Name;
            PlayTime = other.PlayTime;
            IsCleared = other.IsCleared;
            ClearedAt = other.ClearedAt;
            UpdatedAt = other.UpdatedAt;
        }
    }

    public static class RootDataExtensions
    {
        public static IEnumerable<RootData> Clone(this IEnumerable<RootData> original)
        {
            return original?.Select(r => (RootData)r.Clone());
        }
    }
}
