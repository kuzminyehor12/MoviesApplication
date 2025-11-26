using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class MovieEmbeddingsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_MovieEmbeddings_Id",
                table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "MovieEmbeddings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MovieEmbeddings_MovieId",
                table: "MovieEmbeddings",
                column: "MovieId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieEmbeddings_Movies_MovieId",
                table: "MovieEmbeddings",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieEmbeddings_Movies_MovieId",
                table: "MovieEmbeddings");

            migrationBuilder.DropIndex(
                name: "IX_MovieEmbeddings_MovieId",
                table: "MovieEmbeddings");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "MovieEmbeddings");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_MovieEmbeddings_Id",
                table: "Movies",
                column: "Id",
                principalTable: "MovieEmbeddings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
