using Microsoft.EntityFrameworkCore.Migrations;
using Movies.Core.Entities;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddConversions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "keywords",
                table: "Movies",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(Keyword[]),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "genres",
                table: "Movies",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(Genre[]),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "crew",
                table: "Movies",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(CrewMember[]),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "cast",
                table: "Movies",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(CastMember[]),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Keyword[]>(
                name: "keywords",
                table: "Movies",
                type: "jsonb",
                nullable: false,
                defaultValue: new Keyword[0],
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<Genre[]>(
                name: "genres",
                table: "Movies",
                type: "jsonb",
                nullable: false,
                defaultValue: new Genre[0],
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<CrewMember[]>(
                name: "crew",
                table: "Movies",
                type: "jsonb",
                nullable: false,
                defaultValue: new CrewMember[0],
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<CastMember[]>(
                name: "cast",
                table: "Movies",
                type: "jsonb",
                nullable: false,
                defaultValue: new CastMember[0],
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
