using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ErogeDiary.Controls.Histogram;

public delegate string TooltipLabelFormatterDelegate(int bucketIndex);

public class Histogram : Control
{
    private static readonly int GRID_COLUMN_WIDTH = 45;
    private static readonly int HISTOGRAM_AREA_HEIGHT = 150;

    static Histogram()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Histogram),
            new FrameworkPropertyMetadata(typeof(Histogram)));
    }

    public IEnumerable<double>? ItemsSource
    {
        get { return (IEnumerable<double>?)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }
    public static readonly DependencyProperty ItemsSourceProperty =
        register<IEnumerable<double>?>(nameof(ItemsSource));

    // 階級幅
    public double? BucketSize
    {
        get { return (double?)GetValue(BucketSizeProperty); }
        set { SetValue(BucketSizeProperty, value); }
    }
    public static readonly DependencyProperty BucketSizeProperty =
        register<double?>(nameof(BucketSize));

    public TooltipLabelFormatterDelegate? TooltipLabelFormatter
    {
        get { return (TooltipLabelFormatterDelegate?)GetValue(TooltipLabelFormatterProperty); }
        set { SetValue(TooltipLabelFormatterProperty, value); }
    }
    public static readonly DependencyProperty TooltipLabelFormatterProperty =
        register<TooltipLabelFormatterDelegate?>(nameof(TooltipLabelFormatter));

    private static DependencyProperty register<Tprop>(string name) =>
        DependencyProperty.Register(
            name,
            typeof(Tprop),
            typeof(Histogram),
            new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));


    private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as Histogram;
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
        var chartArea = GetTemplateChild("PART_HistogramAreaGrid") as Grid;
        if (chartArea == null)
        {
            return;
        }

        chartArea.ColumnDefinitions.Clear();
        chartArea.Children.Clear();

        // この null check を最初にやると前回のグラフがそのまま残ってしまう
        if (ItemsSource == null || BucketSize == null)
        {
            return;
        }

        var bucketCount = (int)(ItemsSource.Max() / BucketSize) + 1;
        var buckets = new int[bucketCount];

        foreach (var value in ItemsSource)
        {
            var index = (int)(value / BucketSize);
            buckets[index] += 1;
        }

        var maximumBucketValue = buckets.Max();

        // Render buckets
        foreach (var (bucketValue, column) in buckets.WithIndex())
        {
            chartArea.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(GRID_COLUMN_WIDTH, GridUnitType.Pixel)
            });

            var height = (double)bucketValue / maximumBucketValue * HISTOGRAM_AREA_HEIGHT; // 最も高い bucket に合わせて正規化する

            var rect = new Rectangle()
            {
                Height = height,
                ToolTip = $"{bucketValue}回"
            };
            Grid.SetRow(rect, 0);
            Grid.SetColumn(rect, column);
            chartArea.Children.Add(rect);

            var textBlock = new TextBlock()
            {
                Text = TooltipLabelFormatter?.Invoke(column) ?? $"{column}"
            };
            Grid.SetRow(textBlock, 1);
            Grid.SetColumn(textBlock, column);
            chartArea.Children.Add(textBlock);
        }

        // Render horizontal axis
        var chartWidth = GRID_COLUMN_WIDTH * bucketCount;
        var horizontalAxisLine = new Line() { X2 = chartWidth };
        Grid.SetColumnSpan(horizontalAxisLine, bucketCount);
        chartArea.Children.Add(horizontalAxisLine);
    }
}
