using ErogeDiary.Controls.Histogram;
using ErogeDiary.Models.Database.Entities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ErogeDiary.ViewModels.Contents;

public class PlayLogHistogramViewModel : BindableBase
{
    public PlayLogHistogramViewModel(ObservableCollection<PlayLog> playLogs)
    {
        PlayLogs = playLogs;
        Buckets = new List<Bucket>()
        {
            new Bucket(
                Size: TimeSpan.FromMinutes(30).TotalMinutes, 
                Label: "30分", 
                // 0時間～, 0.5時間～, 1時間～ のように表示
                TooltipLabelFormatter: (index) => (index % 2 == 0) switch
                {
                    true => $"{index/2}時間～",
                    false => $"{index/2.0:F1}時間～",
                }
            ),
            new Bucket(TimeSpan.FromHours(1).TotalMinutes, "1時間", (index) => $"{index}時間～"),
        };
        SelectedBucket = Buckets.First();
    }


    private ObservableCollection<PlayLog>? playLogs;
    public ObservableCollection<PlayLog>? PlayLogs
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

    private IEnumerable<Bucket> buckets;
    public IEnumerable<Bucket> Buckets
    {
        get { return buckets; }
        set { SetProperty(ref buckets, value); }
    }

    private Bucket selectedBucket;
    public Bucket SelectedBucket
    {
        get { return selectedBucket; }
        set
        {
            SetProperty(ref selectedBucket, value);
            Update();
        }
    }

    public record Bucket(
        double Size,
        string Label,
        TooltipLabelFormatterDelegate TooltipLabelFormatter
    );

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

    private IEnumerable<double> playTimeMinutesList;
    public IEnumerable<double> PlayTimeMinutesList
    {
        get { return playTimeMinutesList; }
        set { SetProperty(ref playTimeMinutesList, value); }
    }

    private bool hasPlayLogs;
    public bool HasPlayLogs
    {
        get { return hasPlayLogs; }
        set { SetProperty(ref hasPlayLogs, value); }
    }


    private void PlayLogsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => Update();

    private void Update()
    {
        HasPlayLogs = PlayLogs != null && PlayLogs.Count > 0;
        if (!HasPlayLogs)
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

        PlayTimeMinutesList = PlayLogs.Select(p => p.PlayTime.TotalMinutes);
    }
}
