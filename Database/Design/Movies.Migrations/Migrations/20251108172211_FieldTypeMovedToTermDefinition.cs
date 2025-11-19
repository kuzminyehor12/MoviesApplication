using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FieldTypeMovedToTermDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
