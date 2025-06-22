using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAmendmentTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "amendmentTypeId",
                table: "Amendments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AmendmentTypes",
                columns: table => new
                {
                    amendmentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmendmentTypes", x => x.amendmentTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amendments_amendmentTypeId",
                table: "Amendments",
                column: "amendmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amendments_AmendmentTypes_amendmentTypeId",
                table: "Amendments",
                column: "amendmentTypeId",
                principalTable: "AmendmentTypes",
                principalColumn: "amendmentTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amendments_AmendmentTypes_amendmentTypeId",
                table: "Amendments");

            migrationBuilder.DropTable(
                name: "AmendmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Amendments_amendmentTypeId",
                table: "Amendments");

            migrationBuilder.DropColumn(
                name: "amendmentTypeId",
                table: "Amendments");
        }
    }
}
