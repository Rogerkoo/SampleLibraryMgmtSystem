using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleLibraryMgmtSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPublisherField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "BooksEF",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "BooksEF");
        }
    }
}
