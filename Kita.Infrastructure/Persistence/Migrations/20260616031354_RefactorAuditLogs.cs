using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kita.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityKey",
                table: "AuditLogs",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityKey",
                table: "AuditLogs");
        }
    }
}
