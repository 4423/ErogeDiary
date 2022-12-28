using ErogeDiary.Controls.Controls.CalendarHeatmap;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ErogeDiary.Controls.CalendarHeatmap;

public class CalendarHeatmap : Control
{
    private static readonly int CELL_SIZE = 13;
    private static readonly int NUM_OF_WEEKS_IN_YEAR = (int)Math.Ceiling(365.0 / 7.0); // 53
    private static readonly int NUM_OF_DAYS_IN_WEEK = 7;


    static CalendarHeatmap()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarHeatmap),
            new FrameworkPropertyMetadata(typeof(CalendarHeatmap)));
    }


    public IEnumerable<CalendarHeatmapSeries> ItemsSource
    {
        get { return (IEnumerable<CalendarHeatmapSeries>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        register<IEnumerable<CalendarHeatmapSeries>>(nameof(ItemsSource));

    public ColorConverterDelegate ColorConverter
    {
        get { return (ColorConverterDelegate)GetValue(ColorConverterProperty); }
        set { SetValue(ColorConverterProperty, value); }
    }

    public static readonly DependencyProperty ColorConverterProperty =
        register<ColorConverterDelegate>(nameof(ColorConverter));

    public TooltipLabelFormatterDelegate TooltipLabelFormatter
    {
        get { return (TooltipLabelFormatterDelegate)GetValue(TooltipLabelFormatterProperty); }
        set { SetValue(TooltipLabelFormatterProperty, value); }
    }

    public static readonly DependencyProperty TooltipLabelFormatterProperty =
        register<TooltipLabelFormatterDelegate>(nameof(TooltipLabelFormatter));

    private static DependencyProperty register<Tprop>(string name) =>
        DependencyProperty.Register(
            name, 
            typeof(Tprop),
            typeof(CalendarHeatmap),
            new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));


    private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as CalendarHeatmap;
        control?.OnItemsSourceChanged(e.OldValue, e.NewValue);
        control?.UpdateChart();
    }

    private void OnItemsSourceChanged(object oldValue, object newValue)
    {
        var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
        if (oldValueINotifyCollectionChanged != null)
        {
            oldValueINotifyCollectionChanged.CollectionChanged -= ItemsSourceCollectionChanged;
        }

        var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
        if (newValueINotifyCollectionChanged != null)
        {
            newValueINotifyCollectionChanged.CollectionChanged += ItemsSourceCollectionChanged;
        }
    }

    private void ItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateChart();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateChart();
    }

    private void UpdateChart()
    {
        if (ItemsSource == null || ColorConverter == null)
        {
            return;
        }

        var cells = ConvertToCells(ItemsSource);

        RenderMonthLabels(cells);
        RenderChart(cells);
    }

    // 各ゲームの CalendarHeatmapPoint を Grid のどの点に配置するかを表す内部構造
    private record Cell(
        int Row, 
        int Col,
        DateOnly Date,
        List<CalendarHeatmapPoint> CalendarHeatmapPoints
    );

    private IEnumerable<Cell> ConvertToCells(IEnumerable<CalendarHeatmapSeries> heatmapSerieses) {
        // 日ごとのプレイ記録をまとめる
        var pointsByDate = new Dictionary<DateOnly, List<CalendarHeatmapPoint>>();
        foreach (var heatmapSeries in heatmapSerieses)
        {
            foreach (var point in heatmapSeries.Points)
            {
                var points = pointsByDate.GetValueOrDefault(point.Date, new List<CalendarHeatmapPoint>());
                points.Add(point);
                pointsByDate[point.Date] = points;
            }
        }

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);

        // Row/Col に対するプレイ記録に変換
        var totalDaysUntilToday = (NUM_OF_WEEKS_IN_YEAR - 1) * NUM_OF_DAYS_IN_WEEK + ((int)today.DayOfWeek + 1);
        foreach (var offsetDays in Enumerable.Range(0, totalDaysUntilToday))
        {
            int row = offsetDays % NUM_OF_DAYS_IN_WEEK;
            int col = offsetDays / NUM_OF_DAYS_IN_WEEK;

            var date = DateOnly.FromDateTime(now.AddDays(offsetDays - totalDaysUntilToday + 1));

            pointsByDate.TryGetValue(date, out var points);

            yield return new Cell(row, col, date, points ?? new List<CalendarHeatmapPoint>());
        }
    }

    private void RenderMonthLabels(IEnumerable<Cell> cells)
    {
        var monthLabelArea = GetTemplateChild("MonthLabelAreaGrid") as Grid;
        if (monthLabelArea == null)
        {
            return; // 呼び出し元でエラーハンドリングするほどでもない
        }

        monthLabelArea.ColumnDefinitions.Clear();
        monthLabelArea.Children.Clear();

        for (int i = 0; i < NUM_OF_WEEKS_IN_YEAR; i++)
        {
            monthLabelArea.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(CELL_SIZE, GridUnitType.Pixel)
            });
        }

        var firstRowCellsByMonth = cells
            .Where(p => p.Row == 0)
            .GroupBy(p => (p.Date.Year, p.Date.Month));

        foreach (var firstRowCells in firstRowCellsByMonth)
        {
            // 1列分しかない場合は、ラベルが見えにくいので表示しない
            if (firstRowCells.Count() <= 1)
            {
                continue;
            }

            var textBlock = new TextBlock()
            {
                Text = $"{firstRowCells.Key.Month}月"
            };

            var firstColumn = firstRowCells.Min(p => p.Col);
            Grid.SetColumn(textBlock, firstColumn);
            Grid.SetColumnSpan(textBlock, 2);

            monthLabelArea.Children.Add(textBlock);
        }
    }

    private void RenderChart(IEnumerable<Cell> cells)
    {
        var chartArea = GetTemplateChild("ChartAreaGrid") as Grid;
        if (chartArea == null)
        {
            return;
        }

        chartArea.ColumnDefinitions.Clear();
        chartArea.RowDefinitions.Clear();
        chartArea.Children.Clear();

        // 7×53 の枠を定義
        for (int i = 0; i < NUM_OF_WEEKS_IN_YEAR; i++)
        {
            chartArea.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(CELL_SIZE, GridUnitType.Pixel)
            });
        }
        for (int i = 0; i < NUM_OF_DAYS_IN_WEEK; i++)
        {
            chartArea.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(CELL_SIZE, GridUnitType.Pixel)
            });
        }

        foreach (var cell in cells)
        {
            var border = new Border()
            {
                ToolTip = TooltipLabelFormatter.Invoke(cell.Date, cell.CalendarHeatmapPoints),
                Background = ColorConverter.Invoke(cell.CalendarHeatmapPoints),
            };

            Grid.SetRow(border, cell.Row);
            Grid.SetColumn(border, cell.Col);

            chartArea.Children.Add(border);
        }
    }
}
