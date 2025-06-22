using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReportCode",
                table: "AssetLocations",
                type: "VARCHAR(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "Areas",
                type: "NVARCHAR(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AddedById",
                table: "Areas",
                type: "NVARCHAR(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    assetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MdReference = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    ClientReference = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    Position = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    Access = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true),
                    Inaccessable = table.Column<bool>(type: "bit", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: false),
                    assetLocationId = table.Column<int>(type: "int", nullable: false),
                    assetTypeId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.assetId);
                    table.ForeignKey(
                        name: "FK_Assets_AssetLocations_assetLocationId",
                        column: x => x.assetLocationId,
                        principalTable: "AssetLocations",
                        principalColumn: "assetLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypes_assetTypeId",
                        column: x => x.assetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "assetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Clients_clientId",
                        column: x => x.clientId,
                        principalTable: "Clients",
                        principalColumn: "clientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_assetLocationId",
                table: "Assets",
                column: "assetLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_assetTypeId",
                table: "Assets",
                column: "assetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_clientId",
                table: "Assets",
                column: "clientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.AlterColumn<string>(
                name: "ReportCode",
                table: "AssetLocations",
                type: "VARCHAR(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "AddedById",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(450)",
                oldMaxLength: 450);
        }
    }
}
