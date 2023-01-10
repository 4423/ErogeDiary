﻿using System;

namespace ErogeDiary.Models.ErogameScape;

public record GameInfo(
    string Id,
    string Title,
    string Brand,
    DateOnly ReleaseDate,
    string ImageUri
);