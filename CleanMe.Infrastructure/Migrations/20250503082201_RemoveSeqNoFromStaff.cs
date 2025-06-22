using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeqNoFromStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffNo",
                table: "Staff");

            migrationBuilder.RenameColumn(
                name: "BankAccount",
                table: "Staff",
                newName: "BankAccountNumber");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountName",
                table: "Staff",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountName",
                table: "Staff");

            migrationBuilder.RenameColumn(
                name: "BankAccountNumber",
                table: "Staff",
                newName: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "StaffNo",
                table: "Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
