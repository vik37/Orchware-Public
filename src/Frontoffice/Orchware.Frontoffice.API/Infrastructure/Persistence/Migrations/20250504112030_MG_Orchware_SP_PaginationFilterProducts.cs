using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_Orchware_SP_PaginationFilterProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
               CREATE OR ALTER PROCEDURE [dbo].[ProductPaginationFilter]
                        @Page INT,
                        @PageSize INT,
                        @Order NVARCHAR(350) = 'Id',
                        @OrderDirection NVARCHAR(4) = 'ASC',
                        @Filter NVARCHAR(MAX) = '',
                        @SearchTerm NVARCHAR(MAX) = '',
                        @MultiFilter NVARCHAR(MAX) = ''
                    AS
                    BEGIN
                        SET NOCOUNT ON;

                        DECLARE @SQL NVARCHAR(MAX);
                        DECLARE @BaseWhere NVARCHAR(MAX) = '';
                        DECLARE @DynamicParams NVARCHAR(MAX) = 
                            N'@Page INT, @PageSize INT, @SearchTerm NVARCHAR(MAX), @MultiFilter NVARCHAR(MAX), @Order NVARCHAR(350), @OrderDirection NVARCHAR(4)';

                        -- Base filter
                        IF @Filter IS NOT NULL AND LTRIM(RTRIM(@Filter)) <> ''
                            SET @BaseWhere = @Filter;

                        -- MultiFilter (AND)
                        IF @MultiFilter IS NOT NULL AND LTRIM(RTRIM(@MultiFilter)) <> ''
                        BEGIN
                            IF @BaseWhere <> ''
                                SET @BaseWhere = @BaseWhere + ' AND ' + @MultiFilter;
                            ELSE
                                SET @BaseWhere = @MultiFilter;
                        END

                        -- SearchTerm logic (Name LIKE @SearchTerm + '%')
                        IF @SearchTerm IS NOT NULL AND LTRIM(RTRIM(@SearchTerm)) <> ''
                        BEGIN
                            DECLARE @SearchFilter NVARCHAR(MAX) = N'Name LIKE @SearchTerm + ''%''';
                            IF @BaseWhere <> ''
                                SET @BaseWhere = @BaseWhere + ' AND ' + @SearchFilter;
                            ELSE
                                SET @BaseWhere = @SearchFilter;
                        END

                        -- COUNT query
                        SET @SQL = N'SELECT COUNT(*) AS TotalCount FROM [dbo].[Product]';
                        IF @BaseWhere <> ''
                            SET @SQL += ' WHERE ' + @BaseWhere;

                        EXEC sp_executesql 
                            @SQL,
                            @DynamicParams,
                            @Page = @Page,
                            @PageSize = @PageSize,
                            @SearchTerm = @SearchTerm,
                            @MultiFilter = @MultiFilter,
                            @Order = @Order,
                            @OrderDirection = @OrderDirection;

                        -- DATA query
                        SET @SQL = N'SELECT * FROM [dbo].[Product]';
                        IF @BaseWhere <> ''
                            SET @SQL += ' WHERE ' + @BaseWhere;

                        SET @SQL += ' ORDER BY ' + QUOTENAME(@Order) + ' ' + @OrderDirection + '
                            OFFSET (@Page - 1) * @PageSize ROWS
                            FETCH NEXT @PageSize ROWS ONLY';

                        EXEC sp_executesql 
                            @SQL,
                            @DynamicParams,
                            @Page = @Page,
                            @PageSize = @PageSize,
                            @SearchTerm = @SearchTerm,
                            @MultiFilter = @MultiFilter,
                            @Order = @Order,
                            @OrderDirection = @OrderDirection;
                    END;
                    GO
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ProductPaginationFilter')
                    DROP PROCEDURE ProductPaginationFilter;

            ");
        }
    }
}
