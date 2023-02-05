using System;

namespace ErogeDiary.Models;

public record DateRange(
    DateOnly Start,
    DateOnly End,
    string Label
);
