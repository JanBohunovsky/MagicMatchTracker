using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicMatchTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerAlias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "alias",
                table: "players",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alias",
                table: "players");
        }
    }
}
