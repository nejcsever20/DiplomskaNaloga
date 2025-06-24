using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class fixed_save_times_items : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarinskaVrvicva",
                table: "LoadingChecklists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KonecNaklada",
                table: "LoadingChecklists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rampa1",
                table: "LoadingChecklists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rampa2",
                table: "LoadingChecklists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UstreznostVozilca",
                table: "LoadingChecklists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ZacetekNaklada",
                table: "LoadingChecklists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ZavrnilZacetek",
                table: "LoadingChecklists",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarinskaVrvicva",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "KonecNaklada",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "Rampa1",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "Rampa2",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "UstreznostVozilca",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "ZacetekNaklada",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "ZavrnilZacetek",
                table: "LoadingChecklists");
        }
    }
}
