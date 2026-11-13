using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibaryManagement.Migrations
{
    /// <inheritdoc />
    public partial class updateborrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "BorrowDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "BorrowingFee",
                table: "Borrowings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LateFee",
                table: "Borrowings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowingFee",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "LateFee",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Borrowings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BorrowDate",
                table: "Borrowings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
