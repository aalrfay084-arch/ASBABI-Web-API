using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibaryManagement.Migrations
{
    /// <inheritdoc />
    public partial class userId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_AspNetUsers_applicationUserId",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_applicationUserId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "applicationUserId",
                table: "Borrowings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Borrowings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_UserId",
                table: "Borrowings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_AspNetUsers_UserId",
                table: "Borrowings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_AspNetUsers_UserId",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_UserId",
                table: "Borrowings");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Borrowings",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
    }
}
