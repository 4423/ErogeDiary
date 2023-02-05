using ErogeDiary.Controls.CalendarHeatmap;
using ErogeDiary.Controls.Controls.CalendarHeatmap;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using static ErogeDiary.Helpers.DateOnlyHelpers;
using static ErogeDiary.Helpers.MathHelpers;
using static ErogeDiary.Helpers.SolidColorBrushHelpers;

namespace ErogeDiary.ViewModels.Contents;

public class PlayLogsViewModel : BindableBase
{
    private ErogeDiaryDbContext database;

    public PlayLogsViewModel(Game game, ErogeDiaryDbContext database)
    {
        this.database = database;
        Game = game;

        PlayLogDateRanges = new ObservableCollection<DateRange>(BuildPlayLogDateRanges());
        SelectedPlayLogDateRange = PlayLogDateRanges.First();

        Update();
        Game.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Game.TotalPlayTime))
            {
                Update();
            }
        };
    }

    private IEnumerable<DateRange> BuildPlayLogDateRanges()
    {
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);

        // 過去1年（heatmap が日曜始まりなので合わせておくと見栄えがいい）
        var oneYearAgo = now.AddDays(-365);
        var oneYearAgoSunday = oneYearAgo.AddDays(-(int)oneYearAgo.DayOfWeek);
        yield return new DateRange(
            Start: DateOnly.FromDateTime(oneYearAgoSunday),
            End: today,
            Label: "過去1年"
        );

        // PlayLog が存在する年（過去1年と重複する年も含む）
        var years = database.GetPlayLogYears(Game.GameId);
        foreach (var year in years)
        {
            yield return new DateRange(
                Start: new DateOnly(year, 1, 1),
                End: Min(new DateOnly(year, 12, 31), today),
                Label: year.ToString()
            );
        }
    }

    private async void Update()
    {
        var playLogs = database.FindPlayLogsByGameIdAndDateRange(
            gameId: Game.GameId,
            startInclusive: SelectedPlayLogDateRange.Start, 
            endInclusive: SelectedPlayLogDateRange.End
        );
        Series = new ObservableCollection<CalendarHeatmapSeries>
        {
            new CalendarHeatmapSeries(
                Label: Game.Title,
                Points: playLogs.SelectMany(Convert).ToList()
            )
        };

        var firstPlayLog = await database.FindFirstPlayLogByGameId(Game.GameId);
        FirstPlayedAt = firstPlayLog?.StartedAt;
    }

    private IEnumerable<CalendarHeatmapPoint> Convert(PlayLog playLog)
    {
        var startDate = DateOnly.FromDateTime(playLog.StartedAt);
        var endDate = DateOnly.FromDateTime(playLog.EndedAt);

        if (startDate == endDate)
        {
            yield return new CalendarHeatmapPoint(startDate, playLog.PlayTime.TotalHours);
            yield break;
        }

        var lastDateTimeInStartDate = startDate.ToDateTime(TimeOnly.MaxValue);
        var timeSpanOfStart = lastDateTimeInStartDate - playLog.StartedAt;
        yield return new CalendarHeatmapPoint(startDate, timeSpanOfStart.TotalHours);

        for (var dayNumber = startDate.DayNumber + 1; dayNumber < endDate.DayNumber; dayNumber++)
        {
            var intermediateDate = DateOnly.FromDayNumber(dayNumber);
            yield return new CalendarHeatmapPoint(intermediateDate, 24);
        }

        var firstDateTimeInEndDate = endDate.ToDateTime(TimeOnly.MinValue);
        var timeSpanOfEnd = playLog.EndedAt - firstDateTimeInEndDate;
        yield return new CalendarHeatmapPoint(endDate, timeSpanOfEnd.TotalHours);
    }

    private ObservableCollection<CalendarHeatmapSeries>? series;
    public ObservableCollection<CalendarHeatmapSeries>? Series
    {
        get => series;
        set { SetProperty(ref series, value); }
    }

    private Game game;
    public Game Game
    {
        get { return game; }
        set { SetProperty(ref game, value); }
    }

    private DateTime? firstPlayedAt;
    public DateTime? FirstPlayedAt
    {
        get { return firstPlayedAt; }
        set { SetProperty(ref firstPlayedAt, value); }
    }

    private ICollection<DateRange> playLogDateRanges;
    public ICollection<DateRange> PlayLogDateRanges
    {
        get { return playLogDateRanges; }
        set { SetProperty(ref playLogDateRanges, value); }
    }

    private DateRange selectedPlayLogDateRange;
    public DateRange SelectedPlayLogDateRange
    {
        get { return selectedPlayLogDateRange; }
        set
        {
            SetProperty(ref selectedPlayLogDateRange, value);
            Update();
        }
    }

    public ColorConverterDelegate ColorConverter { get; } = (points) =>
    {
        var totalHours = points.Sum(p => p.Value);
        return totalHours switch
        {
            <= 0 => HeatmapColors.Level0,
            < 2 => HeatmapColors.Level1,
            < 4 => HeatmapColors.Level2,
            < 6 => HeatmapColors.Level3,
            >= 6 => HeatmapColors.Level4,
            double.NaN => HeatmapColors.Level0,
        };
    };

    private static class HeatmapColors
    {
        public static SolidColorBrush Level0 { get; } = new SolidColorBrush(Colors.DimGray); // ～0h
        public static SolidColorBrush Level1 { get; } = fromColorCode("#9be9a8"); // 0h～1h
        public static SolidColorBrush Level2 { get; } = fromColorCode("#40c463"); // 2h～3h
        public static SolidColorBrush Level3 { get; } = fromColorCode("#30a14e"); // 4h～5h
        public static SolidColorBrush Level4 { get; } = fromColorCode("#216e39"); // 6h～
    }

    public TooltipLabelFormatterDelegate TooltipLabelFormatter { get; } = (date, points) =>
        $"""
        {date}
        {CeilingWithDecimalPlaces(points.Sum(p => p.Value), 1)}時間
        """;
}