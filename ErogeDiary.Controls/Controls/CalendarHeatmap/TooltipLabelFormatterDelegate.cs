using ErogeDiary.Controls.CalendarHeatmap;
using System;
using System.Collections.Generic;

namespace ErogeDiary.Controls.Controls.CalendarHeatmap;

public delegate string TooltipLabelFormatterDelegate(DateOnly date, IReadOnlyList<CalendarHeatmapPoint> points);
