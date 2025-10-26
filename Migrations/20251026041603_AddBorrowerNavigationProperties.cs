using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleLibraryMgmtSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowerNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BorrowersEF_BookId",
                table: "BorrowersEF",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowersEF_MemberId",
                table: "BorrowersEF",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowersEF_BooksEF_BookId",
                table: "BorrowersEF",
                column: "BookId",
                principalTable: "BooksEF",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowersEF_MembersEF_MemberId",
                table: "BorrowersEF",
                column: "MemberId",
                principalTable: "MembersEF",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowersEF_BooksEF_BookId",
                table: "BorrowersEF");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowersEF_MembersEF_MemberId",
                table: "BorrowersEF");

            migrationBuilder.DropIndex(
                name: "IX_BorrowersEF_BookId",
                table: "BorrowersEF");

            migrationBuilder.DropIndex(
                name: "IX_BorrowersEF_MemberId",
                table: "BorrowersEF");
        }
    }
}
