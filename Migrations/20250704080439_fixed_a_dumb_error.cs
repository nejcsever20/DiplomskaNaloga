using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class fixed_a_dumb_error : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TransportId",
                table: "LoadingChecklists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_LoadingChecklists_TransportId",
                table: "LoadingChecklists",
                column: "TransportId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoadingChecklists_diplomska_Transports_TransportId",
                table: "LoadingChecklists",
                column: "TransportId",
                principalTable: "diplomska_Transports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoadingChecklists_diplomska_Transports_TransportId",
                table: "LoadingChecklists");

            migrationBuilder.DropIndex(
                name: "IX_LoadingChecklists_TransportId",
                table: "LoadingChecklists");

            migrationBuilder.DropColumn(
                name: "TransportId",
                table: "LoadingChecklists");
        }
    }
}
