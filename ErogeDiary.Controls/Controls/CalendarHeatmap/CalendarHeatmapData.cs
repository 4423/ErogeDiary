using System.Collections.Generic;

namespace ErogeDiary.Controls.CalendarHeatmap;

public record CalendarHeatmapData(
    CalendarHeatmapDateRange Range,
    IReadOnlyList<CalendarHeatmapSeries> Series
);
