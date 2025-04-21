using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsDefaultAssetTypeRatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Default",
                table: "AssetTypeRates",
                newName: "IsDefault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "AssetTypeRates",
                newName: "Default");
        }
    }
}
