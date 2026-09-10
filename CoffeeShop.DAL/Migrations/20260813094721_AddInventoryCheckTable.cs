using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryCheckTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discrepancy = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ManagerId = table.Column<int>(type: "integer", nullable: false),
                    CheckDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResolvedTransactionId = table.Column<int>(type: "integer", nullable: true),
                    Explanation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_InventoryItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_InventoryTransactions_ResolvedTransactionId",
                        column: x => x.ResolvedTransactionId,
                        principalTable: "InventoryTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_ItemId",
                table: "InventoryChecks",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_ManagerId",
                table: "InventoryChecks",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_ResolvedTransactionId",
                table: "InventoryChecks",
                column: "ResolvedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_StoreId",
                table: "InventoryChecks",
                column: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryChecks");
        }
    }
}
