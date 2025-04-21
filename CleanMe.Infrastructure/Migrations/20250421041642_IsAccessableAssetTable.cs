using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsAccessableAssetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Inaccessable",
                table: "Assets",
                newName: "IsAccessable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAccessable",
                table: "Assets",
                newName: "Inaccessable");
        }
    }
}
