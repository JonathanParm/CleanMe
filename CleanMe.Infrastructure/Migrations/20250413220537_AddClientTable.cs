using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    clientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: true),
                    AccNo = table.Column<int>(type: "int", nullable: false),
                    Address_Line1 = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: false),
                    Address_Line2 = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: true),
                    Address_Suburb = table.Column<string>(type: "varchar(50)", maxLength: 20, nullable: true),
                    Address_TownOrCity = table.Column<string>(type: "varchar(100)", maxLength: 20, nullable: false),
                    Address_Postcode = table.Column<string>(type: "varchar(10)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "NVARCHAR(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.clientId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
