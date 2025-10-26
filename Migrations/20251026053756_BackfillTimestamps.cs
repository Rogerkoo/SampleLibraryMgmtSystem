using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleLibraryMgmtSystem.Migrations
{
    /// <inheritdoc />
    public partial class BackfillTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill existing rows to the requested static DateAdded/DateUpdated value
            migrationBuilder.Sql("UPDATE \"BooksEF\" SET \"DateAdded\" = '2025-10-26T00:00:00Z' WHERE \"DateAdded\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"BooksEF\" SET \"DateUpdated\" = '2025-10-26T00:00:00Z' WHERE \"DateUpdated\" IS NULL;");

            migrationBuilder.Sql("UPDATE \"MembersEF\" SET \"DateAdded\" = '2025-10-26T00:00:00Z' WHERE \"DateAdded\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"MembersEF\" SET \"DateUpdated\" = '2025-10-26T00:00:00Z' WHERE \"DateUpdated\" IS NULL;");

            migrationBuilder.Sql("UPDATE \"BorrowersEF\" SET \"DateAdded\" = '2025-10-26T00:00:00Z' WHERE \"DateAdded\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"BorrowersEF\" SET \"DateUpdated\" = '2025-10-26T00:00:00Z' WHERE \"DateUpdated\" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: do not revert data back to NULL
            // If necessary, could set back to NULL, but we keep the backfill permanent.
        }
    }
}
