using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class MG_TR_Orders_UpdateProcessingLevel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER [ordering].[TR_Orders_UpdateProcessingLevel]
                ON [ordering].[Order]
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE o
                    SET o.VisibilityLevel = 'warehouseman'
                    FROM [ordering].[Order] as o
                    INNER JOIN inserted i ON o.Id = i.Id
                    INNER JOIN deleted d ON o.Id = d.Id
                    WHERE i.[Status] = 'InPreparation' AND d.[Status] = 'Confirmed';
                END;
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
			migrationBuilder.Sql("DROP TRIGGER IF EXISTS [ordering].[TR_Orders_UpdateProcessingLevel];");
		}
}
