using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace parcial_backend.Migrations
{
    /// <inheritdoc />
    public partial class primera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Estadio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fundacion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EquipoLocalId = table.Column<int>(type: "integer", nullable: false),
                    EquipoVisitanteId = table.Column<int>(type: "integer", nullable: false),
                    GolesLocal = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    GolesVisitante = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                    table.CheckConstraint("CK_Matches_EquiposDiferentes", "\"EquipoLocalId\" != \"EquipoVisitanteId\"");
                    table.ForeignKey(
                        name: "FK_matches_teams_EquipoLocalId",
                        column: x => x.EquipoLocalId,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_matches_teams_EquipoVisitanteId",
                        column: x => x.EquipoVisitanteId,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Posicion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Edad = table.Column<int>(type: "integer", nullable: false),
                    EquipoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_players_teams_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_EquipoLocalId",
                table: "matches",
                column: "EquipoLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_matches_EquipoVisitanteId",
                table: "matches",
                column: "EquipoVisitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_players_EquipoId",
                table: "players",
                column: "EquipoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "teams");
        }
    }
}
