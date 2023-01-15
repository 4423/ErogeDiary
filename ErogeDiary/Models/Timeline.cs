using ErogeDiary.Models.Database.Entities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary.Models
{
    public class Timeline : BindableBase
    {
        private ObservableCollection<PlayLog> playLogs;
        public ObservableCollection<PlayLog> PlayLogs
        {
            get => playLogs;
            set
            {
                if (playLogs != null)
                {
                    playLogs.CollectionChanged -= PlayLogsCollectionChanged;
                }
                if (value != null)
                {
                    value.CollectionChanged += PlayLogsCollectionChanged;
                }
                SetProperty(ref playLogs, value);
                Update();
            }
        }

        private void PlayLogsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => Update();

        private void Update()
        {
            if (PlayLogs == null || PlayLogs.Count == 0)
            {
                return;
            }

            // Median
            int i = PlayLogs.Count / 2;
            if (PlayLogs.Count % 2 == 0)
            {
                MedianPlayTime = (PlayLogs[i].PlayTime + PlayLogs[i - 1].PlayTime) / 2;
            }
            else
            {
                MedianPlayTime = PlayLogs[i].PlayTime;
            }

            // Maximum and Average
            var maximum = TimeSpan.Zero;
            double sumMillisecond = 0;
            foreach (var log in PlayLogs)
            {
                if (maximum < log.PlayTime)
                {
                    maximum = log.PlayTime;
                }
                sumMillisecond += log.PlayTime.TotalMilliseconds;
            }
            MaximumPlayTime = maximum;
            AveragePlayTime = TimeSpan.FromMilliseconds(sumMillisecond / PlayLogs.Count);
        }

        private TimeSpan averagePlayTime;
        public TimeSpan AveragePlayTime
        {
            get => averagePlayTime;
            private set { SetProperty(ref averagePlayTime, value); }
        }

        private TimeSpan medianPlayTime;
        public TimeSpan MedianPlayTime
        {
            get => medianPlayTime;
            private set { SetProperty(ref medianPlayTime, value); }
        }

        private TimeSpan maximumPlayTime;
        public TimeSpan MaximumPlayTime
        {
            get => maximumPlayTime;
            private set { SetProperty(ref maximumPlayTime, value); }
        }
    }
}
