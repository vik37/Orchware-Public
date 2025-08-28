using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class MG_TR_Company_UpdateName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                CREATE OR ALTER TRIGGER TR_Company_UpdateName
                ON [dbo].[Company]
                AFTER UPDATE
                AS
                BEGIN
                    IF UPDATE(Name)
                    BEGIN
                        UPDATE T
                        SET T.[Name] = i.[Name]
                        FROM [dbo].[Company] AS T
                        INNER JOIN deleted AS d ON T.[Name] = d.[Name]
                        INNER JOIN inserted AS i ON d.Id = i.Id
                        WHERE T.[Name] <> i.[Name];
                    END
                END;
                GO
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Company_UpdateName;");
    }
}
