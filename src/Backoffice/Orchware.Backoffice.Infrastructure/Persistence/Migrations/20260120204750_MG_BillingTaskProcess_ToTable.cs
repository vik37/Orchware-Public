using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_BillingTaskProcess_ToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Processing");

            migrationBuilder.CreateTable(
                name: "BillingTask",
                schema: "Processing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsFullPayment = table.Column<bool>(type: "bit", nullable: false),
                    InstallmentId = table.Column<int>(type: "int", nullable: true),
                    InstalmentNumber = table.Column<int>(type: "int", nullable: true),
                    NumberOfInstallments = table.Column<int>(type: "int", nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastErrorMessage = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingTask", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillingTask_Status_InstallmentId",
                schema: "Processing",
                table: "BillingTask",
                columns: new[] { "Status", "InstallmentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingTask",
                schema: "Processing");
        }
    }
}
