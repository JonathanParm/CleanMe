using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockCodeId",
                table: "AssetTypes",
                newName: "stockCodeId");

            migrationBuilder.AddColumn<int>(
                name: "regionId",
                table: "AssetTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StockCodes",
                columns: table => new
                {
                    stockCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCodes", x => x.stockCodeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_stockCodeId",
                table: "AssetTypes",
                column: "stockCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTypes_StockCodes_stockCodeId",
                table: "AssetTypes",
                column: "stockCodeId",
                principalTable: "StockCodes",
                principalColumn: "stockCodeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_StockCodes_stockCodeId",
                table: "AssetTypes");

            migrationBuilder.DropTable(
                name: "StockCodes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTypes_stockCodeId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "regionId",
                table: "AssetTypes");

            migrationBuilder.RenameColumn(
                name: "stockCodeId",
                table: "AssetTypes",
                newName: "StockCodeId");
        }
    }
}
