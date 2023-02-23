using ErogeDiary.Models.Database.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ErogeDiary.SampleData;

public static class SampleData
{
    public static Game Game;
    public static List<Root> Roots;
    public static List<PlayLog> PlayLogsForHeatmap;
    public static List<PlayLog> PlayLogsForHistogram;

    static SampleData()
    {
        Game = new Game()
        {
            GameId = 1000,
            Title = "失われた未来を求めて",
            Brand = "TRUMPLE",
            ReleaseDate = new DateOnly(2010, 11, 26),
            ImageFileName = "waremete.jpg",
            InstallationType = InstallationType.Default,
            ExecutableFilePath = "C:\\waremete.exe",
            RegisteredAt = new DateTime(2020, 1, 1),
            LastPlayedAt = new DateTime(2023, 2, 24),
            ClearedAt = new DateTime(2023, 2, 24),
            TotalPlayTime = TimeSpan.FromMinutes(20*60 + 23),
        };
        Roots = new List<Root>()
        {
            new Root() 
            {
                RootId = 1000_1,
                Name = "佳織",
                PlayTime = TimeSpan.FromHours(8),
                ClearedAt = null,
                Color = (Color)ColorConverter.ConvertFromString("#d46262"),
                GameId = 1000,
            },
            new Root()
            {
                RootId = 1000_2,
                Name = "愛理",
                PlayTime = TimeSpan.FromHours(3.1),
                ClearedAt = null,
                Color = (Color)ColorConverter.ConvertFromString("#336b96"),
                GameId = 1000,
            },
            new Root()
            {
                RootId = 1000_3,
                Name = "凪沙",
                PlayTime = TimeSpan.FromHours(3.6),
                ClearedAt = null,
                Color = (Color)ColorConverter.ConvertFromString("#8463a6"),
                GameId = 1000,
            },
            new Root()
            {
                RootId = 1000_4,
                Name = "ゆい",
                PlayTime = TimeSpan.FromHours(3.3),
                ClearedAt = null,
                Color = (Color)ColorConverter.ConvertFromString("#6389a6"),
                GameId = 1000,
            },
            new Root()
            {
                RootId = 1000_5,
                Name = "TRUE",
                PlayTime = Game.TotalPlayTime - TimeSpan.FromHours(18),
                ClearedAt = null,
                Color = (Color)ColorConverter.ConvertFromString("#d4a862"),
                GameId = 1000,
            }
        };
        PlayLogsForHistogram = new List<PlayLog>()
        {
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 5, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 5, 1, 0, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 8, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 8, 3, 0, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 9, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 9, 1, 0, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 9, 2, 0, 0),
                EndedAt = new DateTime(2022, 10, 9, 3, 34, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 11, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 11, 0, 32, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 15, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 15, 1, 0, 10),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 21, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 21, 4, 32, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 22, 2, 0, 0),
                EndedAt = new DateTime(2022, 10, 22, 2, 50, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 23, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 23, 0, 40, 5),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 23, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 23, 2, 40, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 25, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 25, 0, 32, 7),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 26, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 26, 0, 10, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 27, 1, 0, 0),
                EndedAt = new DateTime(2022, 10, 27, 1, 30, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 10, 30, 0, 0, 0),
                EndedAt = new DateTime(2022, 10, 30, 1, 30, 0),
                GameId = 1000,
            },
            new PlayLog()
            {
                StartedAt = new DateTime(2022, 11, 5, 0, 0, 0),
                EndedAt = new DateTime(2022, 11, 5, 6, 30, 0),
                GameId = 1000,
            },
        };
        // 過去1年間の histogram を適当に埋める用（土日多め）
        PlayLogsForHeatmap = new List<PlayLog>();
        var rand = new Random(0);
        for (int i = 0; i < 370; i++)
        {
            var date = DateTime.Now.AddDays(-i);
            if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                if (rand.Next() % 2 == 0)
                {
                    PlayLogsForHeatmap.Add(new PlayLog()
                    {
                        StartedAt = date.AddHours(-1),
                        EndedAt = date,
                        GameId = 1000,
                    });

                    if (rand.Next() % 5 == 0)
                    {
                        PlayLogsForHeatmap.Add(new PlayLog()
                        {
                            StartedAt = date.AddHours(-2),
                            EndedAt = date,
                            GameId = 1000,
                        });
                    }

                    if (rand.Next() % 3 == 0)
                    {
                        PlayLogsForHeatmap.Add(new PlayLog()
                        {
                            StartedAt = date.AddHours(-2),
                            EndedAt = date,
                            GameId = 1000,
                        });
                    }
                }
            }

            if (rand.Next() % 4 == 0)
            {
                PlayLogsForHeatmap.Add(new PlayLog()
                {
                    StartedAt = date.AddHours(-1),
                    EndedAt = date,
                    GameId = 1000,
                });
            }
        }
    }
}
