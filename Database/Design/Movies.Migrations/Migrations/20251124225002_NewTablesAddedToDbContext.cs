using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class NewTablesAddedToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_MovieEmbedding_Id",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_TermFieldMap_Movies_MovieId",
                table: "TermFieldMap");

            migrationBuilder.DropForeignKey(
                name: "FK_TermFieldMap_Terms_TermId",
                table: "TermFieldMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TermFieldMap",
                table: "TermFieldMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieEmbedding",
                table: "MovieEmbedding");

            migrationBuilder.RenameTable(
                name: "TermFieldMap",
                newName: "TermFieldMaps");

            migrationBuilder.RenameTable(
                name: "MovieEmbedding",
                newName: "MovieEmbeddings");

            migrationBuilder.RenameIndex(
                name: "IX_TermFieldMap_TermId",
                table: "TermFieldMaps",
                newName: "IX_TermFieldMaps_TermId");

            migrationBuilder.RenameIndex(
                name: "IX_TermFieldMap_MovieId",
                table: "TermFieldMaps",
                newName: "IX_TermFieldMaps_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TermFieldMaps",
                table: "TermFieldMaps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieEmbeddings",
                table: "MovieEmbeddings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_MovieEmbeddings_Id",
                table: "Movies",
                column: "Id",
                principalTable: "MovieEmbeddings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TermFieldMaps_Movies_MovieId",
                table: "TermFieldMaps",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TermFieldMaps_Terms_TermId",
                table: "TermFieldMaps",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_MovieEmbeddings_Id",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_TermFieldMaps_Movies_MovieId",
                table: "TermFieldMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_TermFieldMaps_Terms_TermId",
                table: "TermFieldMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TermFieldMaps",
                table: "TermFieldMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieEmbeddings",
                table: "MovieEmbeddings");

            migrationBuilder.RenameTable(
                name: "TermFieldMaps",
                newName: "TermFieldMap");

            migrationBuilder.RenameTable(
                name: "MovieEmbeddings",
                newName: "MovieEmbedding");

            migrationBuilder.RenameIndex(
                name: "IX_TermFieldMaps_TermId",
                table: "TermFieldMap",
                newName: "IX_TermFieldMap_TermId");

            migrationBuilder.RenameIndex(
                name: "IX_TermFieldMaps_MovieId",
                table: "TermFieldMap",
                newName: "IX_TermFieldMap_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TermFieldMap",
                table: "TermFieldMap",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieEmbedding",
                table: "MovieEmbedding",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_MovieEmbedding_Id",
                table: "Movies",
                column: "Id",
                principalTable: "MovieEmbedding",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TermFieldMap_Movies_MovieId",
                table: "TermFieldMap",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TermFieldMap_Terms_TermId",
                table: "TermFieldMap",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
