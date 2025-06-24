using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class added_Items_save_items : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarinskaVrvicva",
                table: "diplomska_Transports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KonecNaklada",
                table: "diplomska_Transports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rampa1",
                table: "diplomska_Transports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rampa2",
                table: "diplomska_Transports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UstreznostVozilca",
                table: "diplomska_Transports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ZacetekNaklada",
                table: "diplomska_Transports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ZavrnilZacetek",
                table: "diplomska_Transports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarinskaVrvicva",
                table: "diplomska_Transports");

            migrationBuilder.DropColumn(
                name: "KonecNaklada",
                table: "diplomska_Transports");

            migrationBuilder.DropColumn(
                name: "Rampa1",
                table: "diplomska_Transports");

            migrationBuilder.DropColumn(
                name: "Rampa2",
                table: "diplomska_Transports");

            migrationBuilder.DropColumn(
                name: "UstreznostVozilca",
                table: "diplomska_Transports");

            migrationBuilder.DropColumn(
                name: "ZacetekNaklada",
                table: "diplomska_Transports");

            migrationBuilder.DropColumn(
                name: "ZavrnilZacetek",
                table: "diplomska_Transports");
        }
    }
}
