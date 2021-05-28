using ErogeDaily.Controls;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public class RootData : BindableBase, ICloneable
    {
        private int id;
        public int Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        private string name;
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }

        private TimeSpan playTime;
        public TimeSpan PlayTime
        {
            get => playTime;
            set { SetProperty(ref playTime, value); }
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
    }

    public static class RootDataExtensions
    {
        public static IEnumerable<RootData> Clone(this IEnumerable<RootData> original)
        {
            return original?.Select(r => (RootData)r.Clone());
        }
    }
}
