using System;
using System.Collections.Generic;

namespace ErogeDiary.Controls.CalendarHeatmap;

public delegate object? CalendarHeatmapCellToolTipSelector(
    DateOnly date,
    IReadOnlyList<CalendarHeatmapPoint> points);
