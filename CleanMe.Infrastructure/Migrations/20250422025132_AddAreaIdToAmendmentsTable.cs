using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    //public partial class AddAreaIdToAmendmentsTable : Migration
    //{
    //    /// <inheritdoc />
    //    protected override void Up(MigrationBuilder migrationBuilder)
    //    {
    //        migrationBuilder.AddColumn<int>(
    //            name: "areaId",
    //            table: "Amendments",
    //            type: "int",
    //            nullable: true);

    //        migrationBuilder.CreateIndex(
    //            name: "IX_Amendments_areaId",
    //            table: "Amendments",
    //            column: "areaId");

    //        migrationBuilder.AddForeignKey(
    //            name: "FK_Amendments_Areas_areaId",
    //            table: "Amendments",
    //            column: "areaId",
    //            principalTable: "Areas",
    //            principalColumn: "areaId",
    //            onDelete: ReferentialAction.Restrict);
    //    }

    //    /// <inheritdoc />
    //    protected override void Down(MigrationBuilder migrationBuilder)
    //    {
    //        migrationBuilder.DropForeignKey(
    //            name: "FK_Amendments_Areas_areaId",
    //            table: "Amendments");

    //        migrationBuilder.DropIndex(
    //            name: "IX_Amendments_areaId",
    //            table: "Amendments");

    //        migrationBuilder.DropColumn(
    //            name: "areaId",
    //            table: "Amendments");
    //    }
    //}
}
