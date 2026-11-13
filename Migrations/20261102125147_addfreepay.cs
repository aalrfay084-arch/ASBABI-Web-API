using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibaryManagement.Migrations
{
    /// <inheritdoc />
    public partial class addfreepay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLateFeePaid",
                table: "Borrowings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LateFeePaidDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLateFeePaid",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "LateFeePaidDate",
                table: "Borrowings");
        }
    }
}
