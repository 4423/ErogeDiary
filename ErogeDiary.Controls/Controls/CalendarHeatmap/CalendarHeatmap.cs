using ErogeDiary.Controls.Controls.CalendarHeatmap;
using ErogeDiary.Controls.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ErogeDiary.Controls.CalendarHeatmap;

public class CalendarHeatmap : Control
{
    private static readonly int CELL_SIZE = 13;
    private static readonly int NUM_OF_DAYS_IN_WEEK = 7;


    static CalendarHeatmap()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarHeatmap),
            new FrameworkPropertyMetadata(typeof(CalendarHeatmap)));
    }


    public CalendarHeatmapData? ChartData
    {
        get { return (CalendarHeatmapData?)GetValue(ChartDataProperty); }
        set { SetValue(ChartDataProperty, value); }
    }

    public static readonly DependencyProperty ChartDataProperty =
        register<CalendarHeatmapData?>(nameof(ChartData));

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
            new PropertyMetadata(new PropertyChangedCallback(OnChartPropertyChanged)));


    private static void OnChartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as CalendarHeatmap;
        control?.UpdateChart();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateChart();
    }

    private void UpdateChart()
    {
        var cells = ChartData == null || ColorConverter == null || TooltipLabelFormatter == null
            ? new List<Cell>()
            : ConvertToCells(ChartData).ToList();

        RenderMonthLabels(cells);
        RenderHeatmap(cells);
    }

    // 各ゲームの CalendarHeatmapPoint を Grid のどの点に配置するかを表す内部構造
    private record Cell(
        int Row, 
        int Col,
        DateOnly Date,
        IReadOnlyList<CalendarHeatmapPoint> CalendarHeatmapPoints
    );

    private IEnumerable<Cell> ConvertToCells(CalendarHeatmapData chartData) {
        if (chartData.Range.Start > chartData.Range.End)
        {
            yield break;
        }

        // 日ごとのプレイ記録をまとめる
        var pointsByDate = new Dictionary<DateOnly, List<CalendarHeatmapPoint>>();
        foreach (var heatmapSeries in chartData.Series)
        {
            foreach (var point in heatmapSeries.Points)
            {
                var points = pointsByDate.GetValueOrDefault(point.Date, new List<CalendarHeatmapPoint>());
                points.Add(point);
                pointsByDate[point.Date] = points;
            }
        }

        // Row/Col に対するプレイ記録に変換
        var daysBetweenStartAndEnd = chartData.Range.End.DayNumber - chartData.Range.Start.DayNumber + 1;
        for (int offsetDays = 0; offsetDays < daysBetweenStartAndEnd; offsetDays++)
        {
            var date = chartData.Range.Start.AddDays(offsetDays);

            int row = (int)date.DayOfWeek;
            int col = ((int)chartData.Range.Start.DayOfWeek + offsetDays) / NUM_OF_DAYS_IN_WEEK;

            pointsByDate.TryGetValue(date, out var points);

            yield return new Cell(row, col, date, points ?? new List<CalendarHeatmapPoint>());
        }
    }

    private void RenderMonthLabels(IReadOnlyCollection<Cell> cells)
    {
        var monthLabelArea = GetTemplateChild("MonthLabelAreaGrid") as Grid;
        if (monthLabelArea == null)
        {
            return; // 呼び出し元でエラーハンドリングするほどでもない
        }

        monthLabelArea.ColumnDefinitions.Clear();
        monthLabelArea.Children.Clear();

        if (cells.Count == 0)
        {
            return;
        }

        int numOfColumn = cells.Max(c => c.Col) + 1;
        for (int i = 0; i < numOfColumn; i++)
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
                Text = string.Format(CultureInfo.CurrentCulture, Strings.CalendarHeatmap_MonthFormat, firstRowCells.Key.Month)
            };

            var firstColumn = firstRowCells.Min(p => p.Col);
            Grid.SetColumn(textBlock, firstColumn);
            Grid.SetColumnSpan(textBlock, 2);

            monthLabelArea.Children.Add(textBlock);
        }
    }

    private void RenderHeatmap(IReadOnlyCollection<Cell> cells)
    {
        var heatmapArea = GetTemplateChild("HeatmapAreaGrid") as Grid;
        if (heatmapArea == null)
        {
            return;
        }

        heatmapArea.ColumnDefinitions.Clear();
        heatmapArea.RowDefinitions.Clear();
        heatmapArea.Children.Clear();

        if (cells.Count == 0)
        {
            return;
        }

        // 曜日×週 の枠を定義
        int numOfColumn = cells.Max(c => c.Col) + 1;
        for (int i = 0; i < numOfColumn; i++)
        {
            heatmapArea.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(CELL_SIZE, GridUnitType.Pixel)
            });
        }
        int numOfRow = cells.Max(c => c.Row) + 1; // NUM_OF_DAYS_IN_WEEK と同じ
        for (int i = 0; i < numOfRow; i++)
        {
            heatmapArea.RowDefinitions.Add(new RowDefinition()
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

            heatmapArea.Children.Add(border);
        }
    }
}
