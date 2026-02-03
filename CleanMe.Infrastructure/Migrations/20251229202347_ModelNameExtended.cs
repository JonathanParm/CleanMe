using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelNameExtended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Regions",
                newName: "RegionName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Clients",
                newName: "ClientName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CleanFrequencies",
                newName: "CleanFrequencyName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Assets",
                newName: "AssetName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Areas",
                newName: "AreaName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AmendmentTypes",
                newName: "AmendmentTypeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegionName",
                table: "Regions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ClientName",
                table: "Clients",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CleanFrequencyName",
                table: "CleanFrequencies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AssetName",
                table: "Assets",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AreaName",
                table: "Areas",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AmendmentTypeName",
                table: "AmendmentTypes",
                newName: "Name");
        }
    }
}
