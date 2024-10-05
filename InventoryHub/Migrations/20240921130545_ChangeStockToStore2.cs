using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryHub.Migrations
{
    public partial class ChangeStockToStore2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockName",
                table: "Store",
                newName: "StoreName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StoreName",
                table: "Store",
                newName: "StockName");
        }
    }
}
