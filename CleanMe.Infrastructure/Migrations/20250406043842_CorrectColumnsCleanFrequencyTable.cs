using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectColumnsCleanFrequencyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "CleanFrequencies",
                newName: "SequencyOrder");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "CleanFrequencies",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "CleanFrequencies",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SequencyOrder",
                table: "CleanFrequencies",
                newName: "SortOrder");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "CleanFrequencies",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "CleanFrequencies",
                newName: "Active");
        }
    }
}
