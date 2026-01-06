using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicMatchTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTurnOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "turn_order",
                table: "match_participations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "turn_order",
                table: "match_participations");
        }
    }
}
