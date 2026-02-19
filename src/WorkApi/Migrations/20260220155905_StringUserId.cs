using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkApi.Migrations
{
    /// <inheritdoc />
    public partial class StringUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_userId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_userId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Tasks",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tasks",
                newName: "userId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_userId",
                table: "Tasks",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_userId",
                table: "Tasks",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
