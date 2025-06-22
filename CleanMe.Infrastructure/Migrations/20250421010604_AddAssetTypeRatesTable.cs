using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetTypeRatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetTypeRates",
                columns: table => new
                {
                    assetTypeRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    assetTypeId = table.Column<int>(type: "int", nullable: false),
                    cleanFrequencyId = table.Column<int>(type: "int", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetTypeRates");
        }
    }
}
