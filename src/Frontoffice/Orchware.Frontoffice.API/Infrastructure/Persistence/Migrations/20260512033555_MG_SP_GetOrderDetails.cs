using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_SP_GetOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[GetOrderDetails]
						@OrderId UNIQUEIDENTIFIER,
						@UserId UNIQUEIDENTIFIER
					AS
					BEGIN
						SET NOCOUNT ON;

						-- Result 1: Order Summary
						SELECT 
							o.OrderNumber, o.[Status], o.OrderDate, o.Discount,
							c.[Name] as CompanyName, c.[Location] as CompanyLocation,
							SUM(od.TotalPrice) as Amount,
							SUM(od.TotalPrice) - (SUM(od.TotalPrice) * o.Discount / 100.0) as TotalAmount,
							SUM(
								CASE od.UnitOfMeasure
									WHEN 't' THEN od.Quantity * 1000.0  
									WHEN 'g' THEN od.Quantity * 0.001
									WHEN 'kg' THEN od.Quantity * 1.0
									ELSE 0.0
								END
							) AS TotalQuantityInKg
						FROM [dbo].[Order] o
						INNER JOIN [dbo].[Company] c ON o.CompanyId = c.Id
						INNER JOIN [dbo].[OrderDetails] od ON o.Id = od.OrderId
						WHERE o.Id = @OrderId AND o.UserId = @UserId
						GROUP BY o.OrderNumber, o.[Status], o.OrderDate, o.Discount, c.[Name], c.[Location];

						-- Result 2: Order Details (Items)
						SELECT pr.[Name] as ProductName, pr.[Image], 
							   od.PricePerUnit, od.Quantity, od.UnitOfMeasure, od.TotalPrice
						FROM [dbo].[OrderDetails] od
						INNER JOIN [dbo].[Product] pr ON od.ProductId = pr.Id
						INNER JOIN [dbo].[Order] o ON o.Id = od.OrderId
						WHERE od.OrderId = @OrderId AND o.UserId = @UserId;

						-- Result 3: Payment & Installments
						SELECT p.WillBePaidInFull, p.NumberOfInstallments, p.AmountPerInstallment, p.PaidInFullDate,
							   i.PaidOnDate, i.IsPaid, i.DueDate, i.Amount
						FROM [dbo].[Payment] p
						LEFT JOIN [dbo].[Installment] i ON i.PaymentId = p.Id
						WHERE p.OrderId = @OrderId AND p.UserId = @UserId;
					END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[GetOrderDetails];");
		}
    }
}
