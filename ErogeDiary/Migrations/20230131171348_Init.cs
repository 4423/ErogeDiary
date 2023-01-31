using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErogeDiary.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ImageFileName = table.Column<string>(type: "TEXT", nullable: false),
                    ErogameScapeGameId = table.Column<string>(type: "TEXT", nullable: true),
                    InstallationType = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutableFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    WindowTitle = table.Column<string>(type: "TEXT", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastPlayedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClearedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TotalPlayTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "PlayLogs",
                columns: table => new
                {
                    PlayLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayLogs", x => x.PlayLogId);
                    table.ForeignKey(
                        name: "FK_PlayLogs_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roots",
                columns: table => new
                {
                    RootId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PlayTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ClearedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roots", x => x.RootId);
                    table.ForeignKey(
                        name: "FK_Roots_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayLogs_GameId",
                table: "PlayLogs",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Roots_GameId",
                table: "Roots",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayLogs");

            migrationBuilder.DropTable(
                name: "Roots");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
