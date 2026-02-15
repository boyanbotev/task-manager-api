using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItem",
                table: "TaskItem");

            migrationBuilder.RenameTable(
                name: "TaskItem",
                newName: "Tasks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "TaskItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItem",
                table: "TaskItem",
                column: "Id");
        }
    }
}
