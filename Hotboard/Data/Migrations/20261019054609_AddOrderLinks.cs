using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkForIncomingPO",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkForMO",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkForOrder",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkForRO",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkForIncomingPO",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LinkForMO",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LinkForOrder",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LinkForRO",
                table: "Orders");
        }
    }
}
