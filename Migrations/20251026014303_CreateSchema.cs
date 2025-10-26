using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleLibraryMgmtSystem.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowersEF",
                table: "BorrowersEF");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BorrowersEF",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowersEF",
                table: "BorrowersEF",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowersEF",
                table: "BorrowersEF");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BorrowersEF");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowersEF",
                table: "BorrowersEF",
                columns: new[] { "MemberId", "BookId" });
        }
    }
}
