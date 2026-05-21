using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.Controls.CalendarHeatmap;

public delegate Brush? CalendarHeatmapCellBrushSelector(
    DateOnly date,
    IReadOnlyList<CalendarHeatmapPoint> points);
