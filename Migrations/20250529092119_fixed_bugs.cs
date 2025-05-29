using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diplomska.Migrations
{
    /// <inheritdoc />
    public partial class fixed_bugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checklistItems_LoadingChecklists_LoadingChecklistid",
                table: "checklistItems");

            migrationBuilder.DropIndex(
                name: "IX_checklistItems_LoadingChecklistid",
                table: "checklistItems");

            migrationBuilder.DropColumn(
                name: "LoadingChecklistid",
                table: "checklistItems");

            migrationBuilder.CreateTable(
                name: "ChecklistAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadingChecklistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswers_LoadingChecklists_LoadingChecklistId",
                        column: x => x.LoadingChecklistId,
                        principalTable: "LoadingChecklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswers_LoadingChecklistId",
                table: "ChecklistAnswers",
                column: "LoadingChecklistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistAnswers");

            migrationBuilder.AddColumn<int>(
                name: "LoadingChecklistid",
                table: "checklistItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_checklistItems_LoadingChecklistid",
                table: "checklistItems",
                column: "LoadingChecklistid");

            migrationBuilder.AddForeignKey(
                name: "FK_checklistItems_LoadingChecklists_LoadingChecklistid",
                table: "checklistItems",
                column: "LoadingChecklistid",
                principalTable: "LoadingChecklists",
                principalColumn: "id");
        }
    }
}
