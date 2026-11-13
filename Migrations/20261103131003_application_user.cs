using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibaryManagement.Migrations
{
    /// <inheritdoc />
    public partial class application_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowerName",
                table: "Borrowings");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Borrowings",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "applicationUserId",
                table: "Borrowings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_applicationUserId",
                table: "Borrowings",
                column: "applicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_AspNetUsers_applicationUserId",
                table: "Borrowings",
                column: "applicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_AspNetUsers_applicationUserId",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_applicationUserId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "applicationUserId",
                table: "Borrowings");

            migrationBuilder.AddColumn<string>(
                name: "BorrowerName",
                table: "Borrowings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
