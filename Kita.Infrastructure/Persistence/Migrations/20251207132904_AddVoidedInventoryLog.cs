using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVoidedInventoryLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "InventoryLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VoidedAt",
                table: "InventoryLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoidedByUserId",
                table: "InventoryLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLogs_VoidedByUserId",
                table: "InventoryLogs",
                column: "VoidedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLogs_AspNetUsers_VoidedByUserId",
                table: "InventoryLogs",
                column: "VoidedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLogs_AspNetUsers_VoidedByUserId",
                table: "InventoryLogs");

            migrationBuilder.DropIndex(
                name: "IX_InventoryLogs_VoidedByUserId",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "VoidedAt",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "VoidedByUserId",
                table: "InventoryLogs");
        }
    }
}
