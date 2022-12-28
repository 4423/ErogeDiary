using ErogeDiary.Controls.CalendarHeatmap;
using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.Controls.Controls.CalendarHeatmap;

// 特定の日付に対する points から色を決定する
public delegate SolidColorBrush ColorConverterDelegate(List<CalendarHeatmapPoint> points);