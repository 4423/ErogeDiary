using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ErogeDaily.Controls
{
    public class HorizontalStackedBarChart : Control
    {
        static HorizontalStackedBarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalStackedBarChart),
                new FrameworkPropertyMetadata(typeof(HorizontalStackedBarChart)));
        }

        public List<SolidColorBrush> Colors { get; set; } = new List<SolidColorBrush>()
        {
            new SolidColorBrush(Color.FromRgb(0x66, 0x51, 0x91)),
            new SolidColorBrush(Color.FromRgb(0xa0, 0x51, 0x95)),
            new SolidColorBrush(Color.FromRgb(0xd4, 0x50, 0x87)),
            new SolidColorBrush(Color.FromRgb(0xf9, 0x5d, 0x6a)),
        };

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

            foreach (var (chartData, i) in ItemsSource.Select((e, i) => (e, i)))
            {
                var columnDefinition = new ColumnDefinition()
                {
                    Width = new GridLength((int)chartData.Value, GridUnitType.Star)
                };
                chartArea.ColumnDefinitions.Add(columnDefinition);

                var textBlock = new TextBlock()
                {
                    Text = chartData.Label,
                    Background = Colors[i],
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

    public class ChartData
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public string ToolTip { get; set; }
    }
}
