using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Movies.Core.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    genres = table.Column<Genre[]>(type: "jsonb", nullable: false),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    Popularity = table.Column<double>(type: "double precision", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TagLine = table.Column<string>(type: "text", nullable: true),
                    cast = table.Column<CastMember[]>(type: "jsonb", nullable: false),
                    crew = table.Column<CrewMember[]>(type: "jsonb", nullable: false),
                    keywords = table.Column<Keyword[]>(type: "jsonb", nullable: false),
                    VoteAverage = table.Column<float>(type: "real", nullable: false),
                    VoteCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FieldType = table.Column<int>(type: "integer", nullable: false),
                    TermText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermIndex",
                columns: table => new
                {
                    TermId = table.Column<int>(type: "integer", nullable: false),
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    TermFrequency = table.Column<float>(type: "real", nullable: false),
                    TermPositions = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermIndex", x => new { x.TermId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_TermIndex_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermIndex_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermVectors",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    TermId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermVectors", x => new { x.TermId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_TermVectors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermVectors_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermIndex_MovieId",
                table: "TermIndex",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TermVectors_MovieId",
                table: "TermVectors",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermIndex");

            migrationBuilder.DropTable(
                name: "TermVectors");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Terms");
        }
    }
}
