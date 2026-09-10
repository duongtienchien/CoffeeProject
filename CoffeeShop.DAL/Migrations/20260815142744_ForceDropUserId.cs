using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ForceDropUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
        name: "UserId",
        table: "InventoryTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
