using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Del = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Worker = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CSR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Courier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
