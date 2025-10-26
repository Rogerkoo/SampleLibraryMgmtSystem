using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleLibraryMgmtSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestamps_Step1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "MembersEF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "MembersEF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "BorrowersEF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "BorrowersEF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "BooksEF",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "BooksEF",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "MembersEF");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "MembersEF");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "BorrowersEF");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "BorrowersEF");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "BooksEF");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "BooksEF");
        }
    }
}
