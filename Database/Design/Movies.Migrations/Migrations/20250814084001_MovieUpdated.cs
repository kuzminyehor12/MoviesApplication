using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class MovieUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Terms_TermText",
                table: "Terms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterPath",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_TermText",
                table: "Terms",
                column: "TermText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Terms_TermText",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "PosterPath",
                table: "Movies");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_TermText",
                table: "Terms",
                column: "TermText",
                unique: true);
        }
    }
}
