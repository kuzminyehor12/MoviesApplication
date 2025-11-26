using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class MovieEmbeddingsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TermVectors_MovieId",
                table: "TermVectors");

            migrationBuilder.DropIndex(
                name: "IX_TermIndex_MovieId",
                table: "TermIndex");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "MovieEmbedding",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vector = table.Column<Vector>(type: "vector(384)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieEmbedding", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermVectors_MovieTerm",
                table: "TermVectors",
                columns: new[] { "MovieId", "TermId" });

            migrationBuilder.CreateIndex(
                name: "IX_Terms_TermType",
                table: "Terms",
                column: "TermType");

            migrationBuilder.CreateIndex(
                name: "IX_TermIndex_MovieTerm",
                table: "TermIndex",
                columns: new[] { "MovieId", "TermId" });

            migrationBuilder.CreateIndex(
                name: "IX_TermFieldMap_FieldMap",
                table: "TermFieldMap",
                column: "FieldType");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_MovieEmbedding_Id",
                table: "Movies",
                column: "Id",
                principalTable: "MovieEmbedding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_MovieEmbedding_Id",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "MovieEmbedding");

            migrationBuilder.DropIndex(
                name: "IX_TermVectors_MovieTerm",
                table: "TermVectors");

            migrationBuilder.DropIndex(
                name: "IX_Terms_TermType",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_TermIndex_MovieTerm",
                table: "TermIndex");

            migrationBuilder.DropIndex(
                name: "IX_TermFieldMap_FieldMap",
                table: "TermFieldMap");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateIndex(
                name: "IX_TermVectors_MovieId",
                table: "TermVectors",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TermIndex_MovieId",
                table: "TermIndex",
                column: "MovieId");
        }
    }
}
