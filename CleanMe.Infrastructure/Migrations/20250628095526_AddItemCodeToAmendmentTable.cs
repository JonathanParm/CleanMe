using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCodeToAmendmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasRegionId",
                table: "AmendmentTypes");

            migrationBuilder.AddColumn<int>(
                name: "itemCodeId",
                table: "Amendments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Amendments_itemCodeId",
                table: "Amendments",
                column: "itemCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amendments_ItemCodes_itemCodeId",
                table: "Amendments",
                column: "itemCodeId",
                principalTable: "ItemCodes",
                principalColumn: "itemCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amendments_ItemCodes_itemCodeId",
                table: "Amendments");

            migrationBuilder.DropIndex(
                name: "IX_Amendments_itemCodeId",
                table: "Amendments");

            migrationBuilder.DropColumn(
                name: "itemCodeId",
                table: "Amendments");

            migrationBuilder.AddColumn<bool>(
                name: "HasRegionId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
