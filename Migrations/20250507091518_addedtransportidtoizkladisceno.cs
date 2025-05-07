using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class addedtransportidtoizkladisceno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TransportId",
                table: "Izkladisceno",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Izkladisceno_TransportId",
                table: "Izkladisceno",
                column: "TransportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Izkladisceno_diplomska_Transports_TransportId",
                table: "Izkladisceno",
                column: "TransportId",
                principalTable: "diplomska_Transports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Izkladisceno_diplomska_Transports_TransportId",
                table: "Izkladisceno");

            migrationBuilder.DropIndex(
                name: "IX_Izkladisceno_TransportId",
                table: "Izkladisceno");

            migrationBuilder.DropColumn(
                name: "TransportId",
                table: "Izkladisceno");
        }
    }
}
