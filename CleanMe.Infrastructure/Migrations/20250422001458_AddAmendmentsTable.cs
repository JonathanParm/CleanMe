using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAmendmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amendments",
                columns: table => new
                {
                    amendmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmendmentSourceName = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    clientId = table.Column<int>(type: "int", nullable: true),
                    areaId = table.Column<int>(type: "int", nullable: true),
                    assetLocationId = table.Column<int>(type: "int", nullable: true),
                    assetId = table.Column<int>(type: "int", nullable: true),
                    staffId = table.Column<int>(type: "int", nullable: true),
                    cleanFrequencyId = table.Column<int>(type: "int", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Access = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true),
                    IsAccessable = table.Column<bool>(type: "bit", nullable: false),
                    StartOn = table.Column<DateOnly>(type: "date", nullable: true),
                    FinishOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Comment = table.Column<string>(type: "VARCHAR(2000)", maxLength: 2000, nullable: true),
                    Invoiced = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amendments", x => x.amendmentId);
                    table.ForeignKey(
                        name: "FK_Amendments_Areas_areaId",
                        column: x => x.areaId,
                        principalTable: "Areas",
                        principalColumn: "areaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amendments_AssetLocations_assetLocationId",
                        column: x => x.assetLocationId,
                        principalTable: "AssetLocations",
                        principalColumn: "assetLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amendments_Assets_assetId",
                        column: x => x.assetId,
                        principalTable: "Assets",
                        principalColumn: "assetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amendments_CleanFrequencies_cleanFrequencyId",
                        column: x => x.cleanFrequencyId,
                        principalTable: "CleanFrequencies",
                        principalColumn: "cleanFrequencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amendments_Clients_clientId",
                        column: x => x.clientId,
                        principalTable: "Clients",
                        principalColumn: "clientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amendments_Staff_staffId",
                        column: x => x.staffId,
                        principalTable: "Staff",
                        principalColumn: "staffId",
                        onDelete: ReferentialAction.Restrict);
                });
 
            migrationBuilder.CreateIndex(
                name: "IX_Amendments_areaId",
                table: "Amendments",
                column: "areaId");

           migrationBuilder.CreateIndex(
                name: "IX_Amendments_assetId",
                table: "Amendments",
                column: "assetId");

            migrationBuilder.CreateIndex(
                name: "IX_Amendments_assetLocationId",
                table: "Amendments",
                column: "assetLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Amendments_cleanFrequencyId",
                table: "Amendments",
                column: "cleanFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Amendments_clientId",
                table: "Amendments",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_Amendments_staffId",
                table: "Amendments",
                column: "staffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amendments");
        }
    }
}
