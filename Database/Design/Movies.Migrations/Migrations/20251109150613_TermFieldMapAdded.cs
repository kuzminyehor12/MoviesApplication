using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class TermFieldMapAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "Terms");

            migrationBuilder.CreateTable(
                name: "TermFieldMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    TermId = table.Column<int>(type: "integer", nullable: false),
                    FieldType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermFieldMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermFieldMap_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermFieldMap_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermFieldMap_MovieId",
                table: "TermFieldMap",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TermFieldMap_TermId",
                table: "TermFieldMap",
                column: "TermId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermFieldMap");

            migrationBuilder.AddColumn<int>(
                name: "FieldType",
                table: "Terms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
