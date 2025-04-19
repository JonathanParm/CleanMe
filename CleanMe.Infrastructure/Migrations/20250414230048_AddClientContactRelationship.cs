using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientContactRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "clientId",
                table: "ClientContacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_clientId",
                table: "ClientContacts",
                column: "clientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientContacts_Clients_clientId",
                table: "ClientContacts",
                column: "clientId",
                principalTable: "Clients",
                principalColumn: "clientId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientContacts_Clients_clientId",
                table: "ClientContacts");

            migrationBuilder.DropIndex(
                name: "IX_ClientContacts_clientId",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "clientId",
                table: "ClientContacts");
        }
    }
}
