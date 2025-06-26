using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class fixed_save_times_items_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarinskaVrvicva",
                table: "Izkladisceno",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KonecNaklada",
                table: "Izkladisceno",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rampa1",
                table: "Izkladisceno",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rampa2",
                table: "Izkladisceno",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UstreznostVozilca",
                table: "Izkladisceno",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ZacetekNaklada",
                table: "Izkladisceno",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ZavrnilZacetek",
                table: "Izkladisceno",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarinskaVrvicva",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "KonecNaklada",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "Rampa1",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "Rampa2",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "UstreznostVozilca",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "ZacetekNaklada",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "ZavrnilZacetek",
                table: "Izkladisceno");
        }
    }
}
