using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FalledLoginAttempts",
                table: "Users",
                newName: "FailedLoginAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "OtpCode",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FailedLoginAttempts",
                table: "Users",
                newName: "FalledLoginAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "OtpCode",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
