using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Users_UserId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventories_Stores_StoreId",
                table: "StoreInventories");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_UserId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InventoryTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventories_ItemId",
                table: "StoreInventories",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StaffId",
                table: "Orders",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_CreateBy",
                table: "InventoryTransactions",
                column: "CreateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Users_CreateBy",
                table: "InventoryTransactions",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_StaffId",
                table: "Orders",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventories_InventoryItems_ItemId",
                table: "StoreInventories",
                column: "ItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventories_Stores_StoreId",
                table: "StoreInventories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Users_CreateBy",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_StaffId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventories_InventoryItems_ItemId",
                table: "StoreInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventories_Stores_StoreId",
                table: "StoreInventories");

            migrationBuilder.DropIndex(
                name: "IX_StoreInventories_ItemId",
                table: "StoreInventories");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StaffId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_CreateBy",
                table: "InventoryTransactions");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "InventoryTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_UserId",
                table: "InventoryTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Users_UserId",
                table: "InventoryTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventories_Stores_StoreId",
                table: "StoreInventories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
