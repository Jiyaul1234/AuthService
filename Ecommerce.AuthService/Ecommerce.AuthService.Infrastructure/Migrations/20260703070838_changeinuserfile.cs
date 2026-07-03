using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeinuserfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "User",
                newName: "MobileNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "User",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "User",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "User",
                newName: "Name");
        }
    }
}
