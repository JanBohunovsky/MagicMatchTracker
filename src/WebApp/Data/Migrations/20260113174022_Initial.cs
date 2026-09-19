using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicMatchTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_started = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    time_ended = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    match_number = table.Column<int>(type: "integer", nullable: false),
                    is_live = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    alias = table.Column<string>(type: "text", nullable: true),
                    avatar_uri = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "decks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    commander = table.Column<string>(type: "text", nullable: false),
                    partner = table.Column<string>(type: "text", nullable: true),
                    colour_identity = table.Column<string>(type: "character varying(5)", nullable: false),
                    image_uri = table.Column<string>(type: "text", nullable: true),
                    deck_uri = table.Column<string>(type: "text", nullable: true),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_decks", x => x.id);
                    table.ForeignKey(
                        name: "fk_decks_players_owner_id",
                        column: x => x.owner_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_participations",
                columns: table => new
                {
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deck_id = table.Column<Guid>(type: "uuid", nullable: true),
                    turn_order = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    end_state_is_winner = table.Column<bool>(type: "boolean", nullable: true),
                    end_state_turn = table.Column<int>(type: "integer", nullable: true),
                    end_state_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_state_lose_condition = table.Column<int>(type: "integer", nullable: true),
                    end_state_killer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_participations", x => new { x.match_id, x.player_id });
                    table.ForeignKey(
                        name: "fk_match_participations_decks_deck_id",
                        column: x => x.deck_id,
                        principalTable: "decks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_match_participations_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_match_participations_players_end_state_killer_id",
                        column: x => x.end_state_killer_id,
                        principalTable: "players",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_match_participations_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_decks_owner_id",
                table: "decks",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_participations_deck_id",
                table: "match_participations",
                column: "deck_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_participations_end_state_killer_id",
                table: "match_participations",
                column: "end_state_killer_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_participations_player_id",
                table: "match_participations",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_participations");

            migrationBuilder.DropTable(
                name: "decks");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
