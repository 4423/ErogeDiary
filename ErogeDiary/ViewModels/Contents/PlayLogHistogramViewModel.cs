using ErogeDiary.Controls.Histogram;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Properties;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace ErogeDiary.ViewModels.Contents;

public partial class PlayLogHistogramViewModel : ObservableObject
{
    public PlayLogHistogramViewModel(ObservableCollection<PlayLog> playLogs)
    {
        PlayLogs = playLogs;
        Buckets = new List<Bucket>()
        {
            new Bucket(
                Size: TimeSpan.FromMinutes(30).TotalMinutes, 
                Label: Strings.PlayLogHistogram_Bucket_30Minutes, 
                // 0時間～, 0.5時間～, 1時間～ のように表示
                TooltipLabelFormatter: (index) => (index % 2 == 0) switch
                {
                    true => string.Format(CultureInfo.CurrentCulture, Strings.PlayLogHistogram_BucketLabelFormat, index / 2),
                    false => string.Format(CultureInfo.CurrentCulture, Strings.PlayLogHistogram_BucketLabelFormat, (index / 2.0).ToString("F1", CultureInfo.CurrentCulture)),
                }
            ),
            new Bucket(TimeSpan.FromHours(1).TotalMinutes, Strings.PlayLogHistogram_Bucket_1Hour, (index) => string.Format(CultureInfo.CurrentCulture, Strings.PlayLogHistogram_BucketLabelFormat, index)),
        };
        SelectedBucket = Buckets.First();
    }


    [ObservableProperty]
    private ObservableCollection<PlayLog>? playLogs;

    partial void OnPlayLogsChanging(ObservableCollection<PlayLog>? value)
    {
        if (playLogs != null)
        {
            playLogs.CollectionChanged -= PlayLogsCollectionChanged;
        }
    }

    partial void OnPlayLogsChanged(ObservableCollection<PlayLog>? value)
    {
        if (value != null)
        {
            value.CollectionChanged += PlayLogsCollectionChanged;
        }
        Update();
    }

    [ObservableProperty]
    private IEnumerable<Bucket> buckets = Enumerable.Empty<Bucket>();

    [ObservableProperty]
    private Bucket selectedBucket = null!;

    partial void OnSelectedBucketChanged(Bucket value)
    {
        Update();
    }

    public record Bucket(
        double Size,
        string Label,
        TooltipLabelFormatterDelegate TooltipLabelFormatter
    );

    [ObservableProperty]
    public partial TimeSpan AveragePlayTime { get; private set; }

    [ObservableProperty]
    public partial TimeSpan MedianPlayTime { get; private set; }

    [ObservableProperty]
    public partial TimeSpan MaximumPlayTime { get; private set; }

    [ObservableProperty]
    private IEnumerable<double>? playTimeMinutesList;

    [ObservableProperty]
    private bool hasPlayLogs;


    private void PlayLogsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => Update();

    private void Update()
    {
        if (PlayLogs is not { Count: > 0 } playLogs)
        {
            HasPlayLogs = false;
            return;
        }
        HasPlayLogs = true;

        // Median
        int i = playLogs.Count / 2;
        if (playLogs.Count % 2 == 0)
        {
            MedianPlayTime = (playLogs[i].PlayTime + playLogs[i - 1].PlayTime) / 2;
        }
        else
        {
            MedianPlayTime = playLogs[i].PlayTime;
        }

        // Maximum and Average
        var maximum = TimeSpan.Zero;
        double sumMillisecond = 0;
        foreach (var log in playLogs)
        {
            if (maximum < log.PlayTime)
            {
                maximum = log.PlayTime;
            }
            sumMillisecond += log.PlayTime.TotalMilliseconds;
        }
        MaximumPlayTime = maximum;
        AveragePlayTime = TimeSpan.FromMilliseconds(sumMillisecond / playLogs.Count);

        PlayTimeMinutesList = playLogs.Select(p => p.PlayTime.TotalMinutes);
    }
}
