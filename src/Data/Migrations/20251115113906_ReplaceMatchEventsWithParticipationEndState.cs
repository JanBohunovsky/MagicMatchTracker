using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicMatchTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMatchEventsWithParticipationEndState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_events");

            migrationBuilder.RenameColumn(
                name: "is_winner",
                table: "match_participations",
                newName: "end_state_is_winner");

            migrationBuilder.AlterColumn<bool>(
                name: "end_state_is_winner",
                table: "match_participations",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<Guid>(
                name: "end_state_killer_id",
                table: "match_participations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "end_state_lose_condition",
                table: "match_participations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "end_state_time",
                table: "match_participations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "end_state_turn",
                table: "match_participations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_match_participations_end_state_killer_id",
                table: "match_participations",
                column: "end_state_killer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_match_participations_players_end_state_killer_id",
                table: "match_participations",
                column: "end_state_killer_id",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_match_participations_players_end_state_killer_id",
                table: "match_participations");

            migrationBuilder.DropIndex(
                name: "ix_match_participations_end_state_killer_id",
                table: "match_participations");

            migrationBuilder.DropColumn(
                name: "end_state_killer_id",
                table: "match_participations");

            migrationBuilder.DropColumn(
                name: "end_state_lose_condition",
                table: "match_participations");

            migrationBuilder.DropColumn(
                name: "end_state_time",
                table: "match_participations");

            migrationBuilder.DropColumn(
                name: "end_state_turn",
                table: "match_participations");

            migrationBuilder.RenameColumn(
                name: "end_state_is_winner",
                table: "match_participations",
                newName: "is_winner");

            migrationBuilder.AlterColumn<bool>(
                name: "is_winner",
                table: "match_participations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "match_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    participation_match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    participation_player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    turn = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_events_match_participations_participation_match_id_pa",
                        columns: x => new { x.participation_match_id, x.participation_player_id },
                        principalTable: "match_participations",
                        principalColumns: new[] { "match_id", "player_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_match_events_participation_match_id_participation_player_id",
                table: "match_events",
                columns: new[] { "participation_match_id", "participation_player_id" });
        }
    }
}
