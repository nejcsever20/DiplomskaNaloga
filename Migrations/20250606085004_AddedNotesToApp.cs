using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotesToApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "diplomska_Transports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "diplomska_Transports");
        }
    }
}
