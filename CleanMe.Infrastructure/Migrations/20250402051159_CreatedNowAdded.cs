using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatedNowAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Staff",
                newName: "AddedById");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Staff",
                newName: "AddedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Regions",
                newName: "AddedById");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Regions",
                newName: "AddedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ErrorExceptionsLogs",
                newName: "OccurredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedById",
                table: "Staff",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "Staff",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "AddedById",
                table: "Regions",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "Regions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "ErrorExceptionsLogs",
                newName: "CreatedAt");
        }
    }
}
