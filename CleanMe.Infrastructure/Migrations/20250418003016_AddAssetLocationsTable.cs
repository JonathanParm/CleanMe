using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetLocationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Regions_regionId",
                table: "Areas");

            migrationBuilder.CreateTable(
                name: "AssetLocations",
                columns: table => new
                {
                    assetLocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false),
                    Address_Line1 = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: false),
                    Address_Line2 = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: true),
                    Address_Suburb = table.Column<string>(type: "varchar(50)", maxLength: 20, nullable: true),
                    Address_TownOrCity = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: false),
                    Address_Postcode = table.Column<string>(type: "varchar(10)", maxLength: 20, nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    ReportCode = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true),
                    AccNo = table.Column<int>(type: "int", nullable: false),
                    areaId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLocations", x => x.assetLocationId);
                    table.ForeignKey(
                        name: "FK_AssetLocations_Areas_areaId",
                        column: x => x.areaId,
                        principalTable: "Areas",
                        principalColumn: "areaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetLocations_areaId",
                table: "AssetLocations",
                column: "areaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Regions_regionId",
                table: "Areas",
                column: "regionId",
                principalTable: "Regions",
                principalColumn: "regionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Regions_regionId",
                table: "Areas");

            migrationBuilder.DropTable(
                name: "AssetLocations");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Regions_regionId",
                table: "Areas",
                column: "regionId",
                principalTable: "Regions",
                principalColumn: "regionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
