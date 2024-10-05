using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryHub.Migrations
{
    public partial class refreshtokenIsExpire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedOn",
                table: "RefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedOn",
                table: "RefreshToken",
                type: "datetime2",
                nullable: true);
        }
    }
}
