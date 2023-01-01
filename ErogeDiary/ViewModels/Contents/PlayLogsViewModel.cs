using ErogeDiary.Controls.CalendarHeatmap;
using ErogeDiary.Controls.Controls.CalendarHeatmap;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using static ErogeDiary.Helpers.MathHelpers;
using static ErogeDiary.Helpers.SolidColorBrushHelpers;

namespace ErogeDiary.ViewModels.Contents;

public class PlayLogsViewModel : BindableBase
{
    private Game game;
    private ErogeDiaryDbContext database;

    public PlayLogsViewModel(Game game, ErogeDiaryDbContext database)
    {
        this.game = game;
        this.database = database;

        Update();
        this.game.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Game.TotalPlayTime))
            {
                Update();
            }
        };
    }

    private void Update()
    {
        var oneYearAgo = DateTime.Now.AddDays(-380); // 1年ちょい前からのデータだけ取得
        var playLogs = database.FindPlayLogsByGameId(game.Id, oneYearAgo);
        Series = new ObservableCollection<CalendarHeatmapSeries>
        {
            new CalendarHeatmapSeries(
                Label: game.Title,
                Points: playLogs.SelectMany(Convert).ToList()
            )
        };
    }

    private IEnumerable<CalendarHeatmapPoint> Convert(PlayLog playLog)
    {
        var startDate = DateOnly.FromDateTime(playLog.StartDateTime);
        var endDate = DateOnly.FromDateTime(playLog.EndDateTime);

        if (startDate == endDate)
        {
            yield return new CalendarHeatmapPoint(startDate, playLog.PlayTime.TotalHours);
            yield break;
        }

        var lastDateTimeInStartDate = startDate.ToDateTime(TimeOnly.MaxValue);
        var timeSpanOfStart = lastDateTimeInStartDate - playLog.StartDateTime;
        yield return new CalendarHeatmapPoint(startDate, timeSpanOfStart.TotalHours);

        for (var dayNumber = startDate.DayNumber + 1; dayNumber < endDate.DayNumber; dayNumber++)
        {
            var intermediateDate = DateOnly.FromDayNumber(dayNumber);
            yield return new CalendarHeatmapPoint(intermediateDate, 24);
        }

        var firstDateTimeInEndDate = endDate.ToDateTime(TimeOnly.MinValue);
        var timeSpanOfEnd = playLog.EndDateTime - firstDateTimeInEndDate;
        yield return new CalendarHeatmapPoint(endDate, timeSpanOfEnd.TotalHours);
    }

    private ObservableCollection<CalendarHeatmapSeries>? series;
    public ObservableCollection<CalendarHeatmapSeries>? Series
    {
        get => series;
        set { SetProperty(ref series, value); }
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