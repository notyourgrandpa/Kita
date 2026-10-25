using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesTransactionCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "ProcessedBy",
                table: "SalesTransactions");

            migrationBuilder.AddColumn<string>(
                name: "ProcessedByUserId",
                table: "SalesTransactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_ProcessedByUserId",
                table: "SalesTransactions",
                column: "ProcessedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ProcessedByUserId",
                table: "SalesTransactions",
                column: "ProcessedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesTransactions_AspNetUsers_ProcessedByUserId",
                table: "SalesTransactions");

            migrationBuilder.DropIndex(
                name: "IX_SalesTransactions_ProcessedByUserId",
                table: "SalesTransactions");

            migrationBuilder.DropColumn(
                name: "ProcessedByUserId",
                table: "SalesTransactions");

            migrationBuilder.AddColumn<string>(
                name: "ProcessedBy",
                table: "SalesTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }
    }
}
