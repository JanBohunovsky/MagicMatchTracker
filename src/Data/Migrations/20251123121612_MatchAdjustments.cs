using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicMatchTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class MatchAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "matches");

            migrationBuilder.AddColumn<bool>(
                name: "is_live",
                table: "matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_live",
                table: "matches");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
