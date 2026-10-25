using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitPriceSubTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ProcessedByUserId",
                table: "SalesTransactions");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "SalesTransactionItems",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "SalesTransactionItems",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ProcessedByUserId",
                table: "SalesTransactions",
                column: "ProcessedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ProcessedByUserId",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "SalesTransactionItems");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "SalesTransactionItems",
                newName: "Price");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ProcessedByUserId",
                table: "SalesTransactions",
                column: "ProcessedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
