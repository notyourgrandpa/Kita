using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSalesTransactionUserFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InventoryLogs_AspNetUsers_ApplicationUserId",
            //    table: "InventoryLogs");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_SalesTransactions_AspNetUsers_ApplicationUserId",
            //    table: "SalesTransactions");

            //migrationBuilder.DropIndex(
            //    name: "IX_SalesTransactions_ApplicationUserId",
            //    table: "SalesTransactions");

            //migrationBuilder.DropIndex(
            //    name: "IX_InventoryLogs_ApplicationUserId",
            //    table: "InventoryLogs");

            //migrationBuilder.DropColumn(
            //    name: "ApplicationUserId",
            //    table: "SalesTransactions");

            //migrationBuilder.DropColumn(
            //    name: "ApplicationUserId",
            //    table: "InventoryLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "SalesTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "InventoryLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_ApplicationUserId",
                table: "SalesTransactions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLogs_ApplicationUserId",
                table: "InventoryLogs",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLogs_AspNetUsers_ApplicationUserId",
                table: "InventoryLogs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ApplicationUserId",
                table: "SalesTransactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
