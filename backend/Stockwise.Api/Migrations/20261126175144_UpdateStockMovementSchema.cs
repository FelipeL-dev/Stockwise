using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stockwise.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStockMovementSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "StockMovements",
                newName: "OldQuantity");

            migrationBuilder.AddColumn<int>(
                name: "Movement",
                table: "StockMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewQuantity",
                table: "StockMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Movement",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "NewQuantity",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "OldQuantity",
                table: "StockMovements",
                newName: "Quantity");
        }
    }
}
