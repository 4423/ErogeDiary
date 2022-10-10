using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ErogeDaily.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImageUri = table.Column<string>(type: "TEXT", nullable: false),
                    ErogameScapeGameId = table.Column<string>(type: "TEXT", nullable: true),
                    InstallationType = table.Column<int>(type: "INTEGER", nullable: false),
                    WindowTitle = table.Column<string>(type: "TEXT", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LatestDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TotalPlayTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IsCleared = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClearedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlayTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PlayTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IsCleared = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClearedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roots_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roots_GameId",
                table: "Roots",
                column: "GameId");
        }

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
