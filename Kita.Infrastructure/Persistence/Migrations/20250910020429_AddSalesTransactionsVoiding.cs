using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesTransactionsVoiding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "SalesTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidedAt",
                table: "SalesTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoidedByUserId",
                table: "SalesTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_VoidedByUserId",
                table: "SalesTransactions",
                column: "VoidedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_VoidedByUserId",
                table: "SalesTransactions",
                column: "VoidedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_VoidedByUserId",
                table: "SalesTransactions");

            migrationBuilder.DropIndex(
                name: "IX_SalesTransactions_VoidedByUserId",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "VoidedAt",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "VoidedByUserId",
                table: "SalesTransactions");
        }
    }
}
