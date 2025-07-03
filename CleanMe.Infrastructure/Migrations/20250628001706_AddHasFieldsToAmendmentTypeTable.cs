using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHasFieldsToAmendmentTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasStaffId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasClientId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRegionId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAreaId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAssetLocationId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasItemCodeId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAssetId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCleanFrequencyId",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRate",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasAccess",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasIsAccessable",
                table: "AmendmentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAccess",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasAreaId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasAssetId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasAssetLocationId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasCleanFrequencyId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasClientId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasIsAccessable",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasItemCodeId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasRate",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasRegionId",
                table: "AmendmentTypes");

            migrationBuilder.DropColumn(
                name: "HasStaffId",
                table: "AmendmentTypes");
        }
    }
}
