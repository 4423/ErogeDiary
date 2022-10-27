using ErogeDiary.Models.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ErogeDiary.Models
{
    public class RootData : VerifiableBindableBase
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
            set
            {
                SetProperty(ref playTime, value);
                ValidateProperty(value);
            }
        }

        private bool isCleared;
        public bool IsCleared
        {
            get => isCleared;
            set
            {
                SetProperty(ref isCleared, value);
                // IsCleared に依存する validation を再評価
                ValidateProperty(ClearedAt, nameof(ClearedAt));
            }
        }

        private DateTime? clearedAt;
        [RequiredIf(nameof(IsCleared), true, ErrorMessage = "攻略日を入力してください。")]
        public DateTime? ClearedAt
        {
            get => clearedAt;
            set
            {
                SetProperty(ref clearedAt, value);
                ValidateProperty(value);
            }
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

        public void Pretty()
        {
            // 整合性を取る
            if (!IsCleared)
            {
                ClearedAt = null;
            }
        }

        public RootData Clone()
        {
            var root = new RootData();
            root.CopyFrom(this);
            return root;
        }

        public void CopyFrom(RootData other)
        {
            Id = other.Id;
            Name = (string)other.Name.Clone();
            PlayTime = other.PlayTime;
            IsCleared = other.IsCleared;
            ClearedAt = other.ClearedAt;
            UpdatedAt = other.UpdatedAt;
        }

        public bool Valid() 
            => !String.IsNullOrWhiteSpace(Name) && !HasErrors;
    }

    public static class RootDataExtensions
    {
        public static IEnumerable<RootData> Clone(this IEnumerable<RootData> original)
        {
            return original.Select(r => r.Clone());
        }
    }
}
