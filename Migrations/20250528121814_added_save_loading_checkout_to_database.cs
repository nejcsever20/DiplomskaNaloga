using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class added_save_loading_checkout_to_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ Conditionally drop the foreign key constraint only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Izkladisceno_diplomska_Transports_TransportId'
                )
                ALTER TABLE Izkladisceno DROP CONSTRAINT FK_Izkladisceno_diplomska_Transports_TransportId;
            ");

            migrationBuilder.AlterColumn<long>(
                name: "TransportId",
                table: "Izkladisceno",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Palete",
                table: "Izkladisceno",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Kolicina",
                table: "Izkladisceno",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "LoadingChecklists",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartLoading = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndLoading = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CmrNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadedQuantity = table.Column<double>(type: "float", nullable: false),
                    RegistrationPlates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverSignature = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadingChecklists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "checklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadingChecklistid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_checklistItems_LoadingChecklists_LoadingChecklistid",
                        column: x => x.LoadingChecklistid,
                        principalTable: "LoadingChecklists",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_checklistItems_LoadingChecklistid",
                table: "checklistItems",
                column: "LoadingChecklistid");

            migrationBuilder.AddForeignKey(
                name: "FK_Izkladisceno_diplomska_Transports_TransportId",
                table: "Izkladisceno",
                column: "TransportId",
                principalTable: "diplomska_Transports",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Izkladisceno_diplomska_Transports_TransportId",
                table: "Izkladisceno");

            migrationBuilder.DropTable(
                name: "checklistItems");

            migrationBuilder.DropTable(
                name: "LoadingChecklists");

            migrationBuilder.AlterColumn<long>(
                name: "TransportId",
                table: "Izkladisceno",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Palete",
                table: "Izkladisceno",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Kolicina",
                table: "Izkladisceno",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Izkladisceno_diplomska_Transports_TransportId",
                table: "Izkladisceno",
                column: "TransportId",
                principalTable: "diplomska_Transports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
