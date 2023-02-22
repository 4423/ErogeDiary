using ErogeDiary.Controls.Helpers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ErogeDiary.Controls;

public class HorizontalStackedBarChart : Control
{
    static HorizontalStackedBarChart()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalStackedBarChart),
            new FrameworkPropertyMetadata(typeof(HorizontalStackedBarChart)));
    }

    public IEnumerable<ChartData> ItemsSource
    {
        get { return (IEnumerable<ChartData>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable<ChartData>),
            typeof(HorizontalStackedBarChart),
            new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

    private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as HorizontalStackedBarChart;
        control?.OnItemsSourceChanged(e.OldValue, e.NewValue);
        control?.UpdateChart();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateChart();
    }

    private void UpdateChart()
    {
        var chartArea = GetTemplateChild("ChartAreaGrid") as Grid;
        if (chartArea == null)
        {
            return;
        }

        chartArea.ColumnDefinitions.Clear();
        chartArea.Children.Clear();

        // この null check を最初にやると前回のグラフがそのまま残ってしまう
        if (ItemsSource == null)
        {
            return;
        }

        var colors = ColorGenerator.Generate(ItemsSource.Count());
        foreach (var ((chartData, color), i) in ItemsSource.Zip(colors, (x, y) => (x, y)).WithIndex())
        {
            var columnDefinition = new ColumnDefinition()
            {
                Width = new GridLength((int)chartData.Value, GridUnitType.Star)
            };
            chartArea.ColumnDefinitions.Add(columnDefinition);

            var textBlock = new TextBlock()
            {
                Text = chartData.Label,
                ToolTip = chartData.ToolTip,
                Background = chartData.Color ?? color,
            };
            Grid.SetColumn(textBlock, i);
            chartArea.Children.Add(textBlock);
        }
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

    private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateChart();
    }
}

internal static class Ext
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> source)
        => source.Select((x, i) => (x, i));
}

public class ChartData
{
    public string Label { get; set; }
    public double Value { get; set; }
    public string ToolTip { get; set; }
    public SolidColorBrush Color { get; set; }
}
