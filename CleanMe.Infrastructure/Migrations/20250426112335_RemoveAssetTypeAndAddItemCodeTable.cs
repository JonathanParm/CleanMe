using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssetTypeAndAddItemCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetTypes_assetTypeId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "AssetTypeRates");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropTable(
                name: "StockCodes");

            migrationBuilder.RenameColumn(
                name: "assetTypeId",
                table: "Assets",
                newName: "itemCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_assetTypeId",
                table: "Assets",
                newName: "IX_Assets_itemCodeId");

            migrationBuilder.RenameColumn(
                name: "Invoiced",
                table: "Amendments",
                newName: "InvoicedOn");

            migrationBuilder.CreateTable(
                name: "ItemCodes",
                columns: table => new
                {
                    itemCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    ItemName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ItemDescription = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    PurchasesDescription = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    SalesDescription = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    PurchasesUnitRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SalesUnitRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PurchasesXeroAccount = table.Column<int>(type: "int", nullable: true),
                    SalesXeroAccount = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCodes", x => x.itemCodeId);
                });

            migrationBuilder.CreateTable(
                name: "ItemCodeRates",
                columns: table => new
                {
                    itemCodeRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    itemCodeId = table.Column<int>(type: "int", nullable: false),
                    cleanFrequencyId = table.Column<int>(type: "int", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCodeRates", x => x.itemCodeRateId);
                    table.ForeignKey(
                        name: "FK_ItemCodeRates_CleanFrequencies_cleanFrequencyId",
                        column: x => x.cleanFrequencyId,
                        principalTable: "CleanFrequencies",
                        principalColumn: "cleanFrequencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCodeRates_Clients_clientId",
                        column: x => x.clientId,
                        principalTable: "Clients",
                        principalColumn: "clientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCodeRates_ItemCodes_itemCodeId",
                        column: x => x.itemCodeId,
                        principalTable: "ItemCodes",
                        principalColumn: "itemCodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemCodeRates_cleanFrequencyId",
                table: "ItemCodeRates",
                column: "cleanFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCodeRates_clientId",
                table: "ItemCodeRates",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCodeRates_itemCodeId",
                table: "ItemCodeRates",
                column: "itemCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_ItemCodes_itemCodeId",
                table: "Assets",
                column: "itemCodeId",
                principalTable: "ItemCodes",
                principalColumn: "itemCodeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_ItemCodes_itemCodeId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "ItemCodeRates");

            migrationBuilder.DropTable(
                name: "ItemCodes");

            migrationBuilder.RenameColumn(
                name: "itemCodeId",
                table: "Assets",
                newName: "assetTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_itemCodeId",
                table: "Assets",
                newName: "IX_Assets_assetTypeId");

            migrationBuilder.RenameColumn(
                name: "InvoicedOn",
                table: "Amendments",
                newName: "Invoiced");

            migrationBuilder.CreateTable(
                name: "StockCodes",
                columns: table => new
                {
                    stockCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCodes", x => x.stockCodeId);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    assetTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stockCodeId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    regionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.assetTypeId);
                    table.ForeignKey(
                        name: "FK_AssetTypes_StockCodes_stockCodeId",
                        column: x => x.stockCodeId,
                        principalTable: "StockCodes",
                        principalColumn: "stockCodeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypeRates",
                columns: table => new
                {
                    assetTypeRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    assetTypeId = table.Column<int>(type: "int", nullable: false),
                    cleanFrequencyId = table.Column<int>(type: "int", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypeRates", x => x.assetTypeRateId);
                    table.ForeignKey(
                        name: "FK_AssetTypeRates_AssetTypes_assetTypeId",
                        column: x => x.assetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "assetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetTypeRates_CleanFrequencies_cleanFrequencyId",
                        column: x => x.cleanFrequencyId,
                        principalTable: "CleanFrequencies",
                        principalColumn: "cleanFrequencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetTypeRates_Clients_clientId",
                        column: x => x.clientId,
                        principalTable: "Clients",
                        principalColumn: "clientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypeRates_assetTypeId",
                table: "AssetTypeRates",
                column: "assetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypeRates_cleanFrequencyId",
                table: "AssetTypeRates",
                column: "cleanFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypeRates_clientId",
                table: "AssetTypeRates",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_stockCodeId",
                table: "AssetTypes",
                column: "stockCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetTypes_assetTypeId",
                table: "Assets",
                column: "assetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "assetTypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
