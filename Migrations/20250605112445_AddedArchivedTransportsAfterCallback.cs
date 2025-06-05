using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class AddedArchivedTransportsAfterCallback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedTransports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sp = table.Column<bool>(type: "bit", nullable: false),
                    SK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Skladisce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VrstaTransporta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StTransporta = table.Column<long>(type: "bigint", nullable: true),
                    PlaniranPrihod = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PavzaVoznika = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DolocenSkladiscnikId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registracija = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VrstaPrevoznegaSredstva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Voznik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NAVISZacetekSklada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NAVISKonecSklada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCallback = table.Column<bool>(type: "bit", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedTransports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedTransports");
        }
    }
}
