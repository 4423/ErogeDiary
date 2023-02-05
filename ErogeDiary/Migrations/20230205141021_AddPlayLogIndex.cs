using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErogeDiary.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayLogIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayLogs_GameId",
                table: "PlayLogs");

            migrationBuilder.CreateIndex(
                name: "IX_PlayLogs_GameId_StartedAt",
                table: "PlayLogs",
                columns: new[] { "GameId", "StartedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayLogs_GameId_StartedAt",
                table: "PlayLogs");

            migrationBuilder.CreateIndex(
                name: "IX_PlayLogs_GameId",
                table: "PlayLogs",
                column: "GameId");
        }
    }
}
