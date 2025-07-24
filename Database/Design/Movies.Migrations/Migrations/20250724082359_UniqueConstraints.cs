using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "Terms");

            migrationBuilder.AddColumn<int>(
                name: "FieldType",
                table: "TermVectors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_TermText",
                table: "Terms",
                column: "TermText",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Terms_TermText",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Title",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "TermVectors");

            migrationBuilder.AddColumn<int>(
                name: "FieldType",
                table: "Terms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
