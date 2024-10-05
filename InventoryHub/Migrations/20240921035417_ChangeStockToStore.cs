using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryHub.Migrations
{
    public partial class ChangeStockToStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Stock_StockId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "Products",
                newName: "StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_StockId",
                table: "Products",
                newName: "IX_Products_StoreId");

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Store_StoreId",
                table: "Products",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Store_StoreId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "Products",
                newName: "StockId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_StoreId",
                table: "Products",
                newName: "IX_Products_StockId");

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Stock_StockId",
                table: "Products",
                column: "StockId",
                principalTable: "Stock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
