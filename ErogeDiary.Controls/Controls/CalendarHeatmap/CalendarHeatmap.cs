using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ErogeDiary.Controls.CalendarHeatmap;

public class CalendarHeatmap : Control
{
    private const int CELL_SIZE = 13;
    private const int NUM_OF_DAYS_IN_WEEK = 7;

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
        RegisterChartProperty<CalendarHeatmapData?>(nameof(ChartData));

    public DayOfWeek? FirstDayOfWeek
    {
        get { return (DayOfWeek?)GetValue(FirstDayOfWeekProperty); }
        set { SetValue(FirstDayOfWeekProperty, value); }
    }

    public static readonly DependencyProperty FirstDayOfWeekProperty =
        RegisterChartProperty<DayOfWeek?>(nameof(FirstDayOfWeek));

    public CalendarHeatmapDayLabelVisibility DayLabelVisibility
    {
        get { return (CalendarHeatmapDayLabelVisibility)GetValue(DayLabelVisibilityProperty); }
        set { SetValue(DayLabelVisibilityProperty, value); }
    }

    public static readonly DependencyProperty DayLabelVisibilityProperty =
        DependencyProperty.Register(
            nameof(DayLabelVisibility),
            typeof(CalendarHeatmapDayLabelVisibility),
            typeof(CalendarHeatmap),
            new PropertyMetadata(CalendarHeatmapDayLabelVisibility.Sparse, new PropertyChangedCallback(OnChartPropertyChanged)));

    public CalendarHeatmapCellBrushSelector? CellBrushSelector
    {
        get { return (CalendarHeatmapCellBrushSelector?)GetValue(CellBrushSelectorProperty); }
        set { SetValue(CellBrushSelectorProperty, value); }
    }

    public static readonly DependencyProperty CellBrushSelectorProperty =
        RegisterChartProperty<CalendarHeatmapCellBrushSelector?>(nameof(CellBrushSelector));

    public CalendarHeatmapCellToolTipSelector? CellToolTipSelector
    {
        get { return (CalendarHeatmapCellToolTipSelector?)GetValue(CellToolTipSelectorProperty); }
        set { SetValue(CellToolTipSelectorProperty, value); }
    }

    public static readonly DependencyProperty CellToolTipSelectorProperty =
        RegisterChartProperty<CalendarHeatmapCellToolTipSelector?>(nameof(CellToolTipSelector));

    private static DependencyProperty RegisterChartProperty<Tprop>(string name) =>
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

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == LanguageProperty)
        {
            UpdateChart();
        }
    }

    private void UpdateChart()
    {
        var labelCulture = GetEffectiveLabelCulture();
        var firstDayOfWeek = GetEffectiveFirstDayOfWeek();
        var cells = ChartData == null
            ? []
            : ConvertToCells(ChartData, firstDayOfWeek).ToList();

        RenderDayLabels(labelCulture, firstDayOfWeek);
        RenderMonthLabels(cells, labelCulture);
        RenderHeatmap(cells);
    }

    // 各ゲームの CalendarHeatmapPoint を Grid のどの点に配置するかを表す内部構造
    private record Cell(
        int Row,
        int Col,
        DateOnly Date,
        IReadOnlyList<CalendarHeatmapPoint> Points
    );

    private IEnumerable<Cell> ConvertToCells(CalendarHeatmapData chartData, DayOfWeek firstDayOfWeek)
    {
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
        var startDayOffset = GetDayOffset(chartData.Range.Start.DayOfWeek, firstDayOfWeek);
        for (int offsetDays = 0; offsetDays < daysBetweenStartAndEnd; offsetDays++)
        {
            var date = chartData.Range.Start.AddDays(offsetDays);

            int row = GetDayOffset(date.DayOfWeek, firstDayOfWeek);
            int col = (startDayOffset + offsetDays) / NUM_OF_DAYS_IN_WEEK;

            pointsByDate.TryGetValue(date, out var points);

            yield return new Cell(row, col, date, points ?? new List<CalendarHeatmapPoint>());
        }
    }

    private void RenderDayLabels(CultureInfo culture, DayOfWeek firstDayOfWeek)
    {
        var dayLabelArea = GetTemplateChild("DayLabelAreaGrid") as Grid;
        if (dayLabelArea == null)
        {
            return;
        }

        dayLabelArea.RowDefinitions.Clear();
        dayLabelArea.Children.Clear();

        for (int i = 0; i < NUM_OF_DAYS_IN_WEEK; i++)
        {
            dayLabelArea.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(CELL_SIZE, GridUnitType.Pixel)
            });

            if (!ShouldRenderDayLabel(i))
            {
                continue;
            }

            var dayOfWeek = AddDays(firstDayOfWeek, i);
            var textBlock = new TextBlock()
            {
                Text = culture.DateTimeFormat.ShortestDayNames[(int)dayOfWeek],
            };
            Grid.SetRow(textBlock, i);
            dayLabelArea.Children.Add(textBlock);
        }
    }

    private void RenderMonthLabels(IReadOnlyCollection<Cell> cells, CultureInfo culture)
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
                Text = new DateOnly(firstRowCells.Key.Year, firstRowCells.Key.Month, 1).ToString("MMM", culture)
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
                ToolTip = CellToolTipSelector?.Invoke(cell.Date, cell.Points),
                Background = CellBrushSelector?.Invoke(cell.Date, cell.Points) ?? SystemColors.ControlDarkBrush,
            };

            Grid.SetRow(border, cell.Row);
            Grid.SetColumn(border, cell.Col);

            heatmapArea.Children.Add(border);
        }
    }

    private CultureInfo GetEffectiveLabelCulture()
    {
        var languageValueSource = DependencyPropertyHelper.GetValueSource(this, LanguageProperty);
        if (languageValueSource.BaseValueSource == BaseValueSource.Default)
        {
            return CultureInfo.CurrentUICulture;
        }

        try
        {
            return Language.GetSpecificCulture();
        }
        catch (InvalidOperationException)
        {
            return CultureInfo.CurrentUICulture;
        }
    }

    private DayOfWeek GetEffectiveFirstDayOfWeek() =>
        FirstDayOfWeek ?? CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

    private static int GetDayOffset(DayOfWeek dayOfWeek, DayOfWeek firstDayOfWeek) =>
        ((int)dayOfWeek - (int)firstDayOfWeek + NUM_OF_DAYS_IN_WEEK) % NUM_OF_DAYS_IN_WEEK;

    private static DayOfWeek AddDays(DayOfWeek dayOfWeek, int days) =>
        (DayOfWeek)(((int)dayOfWeek + days) % NUM_OF_DAYS_IN_WEEK);

    private bool ShouldRenderDayLabel(int row) =>
        DayLabelVisibility switch
        {
            CalendarHeatmapDayLabelVisibility.None => false,
            CalendarHeatmapDayLabelVisibility.Sparse => row is 1 or 3 or 5,
            CalendarHeatmapDayLabelVisibility.All => true,
            _ => false,
        };
}
