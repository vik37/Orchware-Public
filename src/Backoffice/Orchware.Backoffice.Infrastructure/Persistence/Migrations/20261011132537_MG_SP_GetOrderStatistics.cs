using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_SP_GetOrderStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [ordering].[SP_GetOrderStatistics]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- 1. OVERALL ORDER STATISTICS
                    SELECT 
                            COUNT(*) AS TotalOrders,
                            COALESCE(SUM(o.TotalAmount), 0) AS TotalOrderAmount,
                            COALESCE(AVG(o.TotalAmount), 0) AS AverageOrderAmount,
                            firstOrder.OrderNumber AS FirstOrderNumber,
                            firstOrder.OrderedDate AS FirstOrderDate,
                            latestOrder.OrderNumber AS LatestOrderNumber,
                            latestOrder.OrderedDate AS LatestOrderDate
                        FROM [ordering].[Order] o
                        OUTER APPLY
                        (
                            SELECT TOP (1)
                                o2.OrderNumber,
                                o2.OrderedDate
                            FROM [ordering].[Order] o2
                            WHERE o2.[Status] <> 'Cancelled'
                            ORDER BY o2.OrderedDate ASC, o2.OrderNumber ASC
                        ) firstOrder
                        OUTER APPLY
                        (
                            SELECT TOP (1)
                                o3.OrderNumber,
                                o3.OrderedDate
                            FROM [ordering].[Order] o3
                            WHERE o3.[Status] <> 'Cancelled'
                            ORDER BY o3.OrderedDate DESC, o3.OrderNumber DESC
                        ) latestOrder
                        WHERE o.[Status] <> 'Cancelled'
                        GROUP BY
                            firstOrder.OrderNumber,
                            firstOrder.OrderedDate,
                            latestOrder.OrderNumber,
                            latestOrder.OrderedDate;

                    -- 2. COMPANY STATISTICS
                    SELECT
                        o.Buyer_CompanyName AS CompanyName,
                        COUNT(*) AS OrderCount,
                        COALESCE(SUM(o.TotalAmount), 0) AS TotalOrderAmount,
                        COALESCE(AVG(o.TotalAmount), 0) AS AverageOrderAmount
                    FROM [ordering].[Order] o
                    WHERE o.[Status] <> 'Cancelled'
                    GROUP BY o.Buyer_CompanyName
                    ORDER BY COUNT(*) DESC, SUM(o.TotalAmount) DESC;

                    -- 3. BUYER + COMPANY STATISTICS
                    SELECT
                        o.Buyer_Name AS BuyerName,
                        o.Buyer_PersonalEmail AS PersonalEmail,
                        o.Buyer_CompanyName AS CompanyName,
                        COUNT(*) AS OrderCount,
                        COALESCE(SUM(o.TotalAmount), 0) AS TotalOrderAmount,
                        COALESCE(AVG(o.TotalAmount), 0) AS AverageOrderAmount
                    FROM [ordering].[Order] o
                    WHERE o.[Status] <> 'Cancelled'
                    GROUP BY o.Buyer_Name, o.Buyer_PersonalEmail, o.Buyer_CompanyName
                    ORDER BY COUNT(*) DESC, SUM(o.TotalAmount) DESC;

                    -- 4. PAYMENT STATISTICS
                    WITH PaymentSummary AS (
                        SELECT
                            o.Id AS OrderId,
                            o.TotalAmount,
                            p.WillBePaidInFull,
                            p.IsPaidInFull,
                            COALESCE(inst.PaidAmount, 0) AS PaidAmount,
                            COALESCE(inst.InstallmentCount, 0) AS InstallmentCount,
                            COALESCE(inst.PaidInstallmentCount, 0) AS PaidInstallmentCount
                        FROM [ordering].[Order] o
                        INNER JOIN [ordering].[PaymentInfo] p ON p.OrderId = o.Id
                        OUTER APPLY (
                            SELECT
                                SUM(CASE WHEN i.IsPaid = 1 THEN i.Amount ELSE 0 END) AS PaidAmount,
                                COUNT(*) AS InstallmentCount,
                                SUM(CASE WHEN i.IsPaid = 1 THEN 1 ELSE 0 END) AS PaidInstallmentCount
                            FROM [ordering].[Installment] i
                            WHERE i.PaymentInfoId = p.Id
                        ) inst
                        WHERE o.[Status] <> 'Cancelled'
                    ),
                    PaymentStatus AS (
                        SELECT *,
                            CASE
                                WHEN WillBePaidInFull = 1 THEN CASE WHEN IsPaidInFull = 1 THEN 1 ELSE 0 END
                                WHEN InstallmentCount > 0 AND PaidInstallmentCount = InstallmentCount THEN 1
                                ELSE 0
                            END AS IsFullyPaid,
                            CASE
                                WHEN WillBePaidInFull = 1 AND IsPaidInFull = 1 THEN TotalAmount
                                WHEN WillBePaidInFull = 1 THEN 0
                                ELSE PaidAmount
                            END AS ActualPaidAmount
                        FROM PaymentSummary
                    )
                    SELECT
                        COUNT(*) AS TotalOrders,
                        SUM(IsFullyPaid) AS FullyPaidOrders,
                        COALESCE(SUM(CASE WHEN IsFullyPaid = 1 THEN TotalAmount ELSE 0 END), 0) AS FullyPaidOrderValue,
                        COALESCE(SUM(ActualPaidAmount), 0) AS TotalActuallyPaid,
                        COALESCE(SUM(CASE WHEN TotalAmount > ActualPaidAmount THEN TotalAmount - ActualPaidAmount ELSE 0 END), 0) AS OutstandingBalance,
                        SUM(CASE WHEN WillBePaidInFull = 1 THEN 1 ELSE 0 END) AS PayInFullOrders,
                        SUM(CASE WHEN WillBePaidInFull = 0 THEN 1 ELSE 0 END) AS InstallmentOrders
                    FROM PaymentStatus;

                    -- 5. CANCELLATION STATISTICS
                    SELECT
                        CASE o.CancelledBy 
							WHEN 0 THEN 'None'
							WHEN 1 THEN 'Client'
							WHEN 2 THEN 'Manager'
							ELSE 'Unknown'
						END AS CancelledBy,
                        COUNT(*) AS CancelledOrderCount,
                        COALESCE(SUM(o.TotalAmount), 0) AS CancelledOrderValue
                    FROM [ordering].[Order] o
                    WHERE o.[Status] = 'Cancelled'
                    GROUP BY o.CancelledBy
                    ORDER BY COUNT(*) DESC;

                    -- 6. TOP PRODUCT BY QUANTITY (KG)
                    WITH ProductSummary AS (
                        SELECT
                            p.[Name] AS ProductName,
                            p.[Image] AS ProductImage,
                            COUNT(DISTINCT o.Id) AS OrderCount,
                            SUM(CASE od.UnitOfMeasure WHEN 't' THEN od.Quantity * 1000.0 WHEN 'kg' THEN od.Quantity WHEN 'g' THEN od.Quantity * 0.001 ELSE 0.0 END) AS TotalQuantityKg,
                            SUM(od.TotalPrice) AS TotalSalesAmount
                        FROM [ordering].[OrderDetails] od
                        INNER JOIN [ordering].[Order] o ON o.Id = od.OrderId
                        INNER JOIN [inventory].[Product] p ON p.Id = od.ShelfProductId
                        WHERE o.[Status] <> 'Cancelled'
                        GROUP BY p.[Name], p.[Image]
                    )
                    SELECT TOP (1) ProductName, ProductImage, OrderCount, TotalQuantityKg, TotalSalesAmount
                    FROM ProductSummary
                    ORDER BY TotalQuantityKg DESC;

                    -- 7. TOP PRODUCT BY ORDER COUNT
                    WITH ProductSummary AS (
                        SELECT
                            p.Id AS ProductId,
                            p.[Name] AS ProductName,
                            p.[Image] AS ProductImage,
                            COUNT(DISTINCT o.Id) AS OrderCount,
                            SUM(CASE od.UnitOfMeasure WHEN 't' THEN od.Quantity * 1000.0 WHEN 'kg' THEN od.Quantity WHEN 'g' THEN od.Quantity * 0.001 ELSE 0.0 END) AS TotalQuantityKg,
                            SUM(od.TotalPrice) AS TotalSalesAmount
                        FROM [ordering].[OrderDetails] od
                        INNER JOIN [ordering].[Order] o ON o.Id = od.OrderId
                        INNER JOIN [inventory].[Product] p ON p.Id = od.ShelfProductId
                        WHERE o.[Status] <> 'Cancelled'
                        GROUP BY p.Id, p.[Name], p.[Image]
                    )
                    SELECT TOP (1) ProductId, ProductName, ProductImage, OrderCount, TotalQuantityKg, TotalSalesAmount
                    FROM ProductSummary
                    ORDER BY OrderCount DESC;

                    -- 8. UNIQUE BUYERS AND COMPANIES
                    SELECT
                        COUNT(DISTINCT o.Buyer_PersonalEmail) AS UniqueBuyers,
                        COUNT(DISTINCT o.Buyer_CompanyName) AS UniqueCompanies
                    FROM [ordering].[Order] o
                    WHERE o.[Status] <> 'Cancelled';

                    -- 9. REPEAT BUYERS
                    SELECT
                        o.Buyer_Name AS BuyerName,
                        o.Buyer_PersonalEmail AS PersonalEmail,
                        o.Buyer_CompanyName AS CompanyName,
                        COUNT(*) AS OrderCount,
                        SUM(o.TotalAmount) AS TotalOrderAmount,
                        AVG(o.TotalAmount) AS AverageOrderAmount
                    FROM [ordering].[Order] o
                    WHERE o.[Status] <> 'Cancelled'
                    GROUP BY o.Buyer_Name, o.Buyer_PersonalEmail, o.Buyer_CompanyName
                    HAVING COUNT(*) > 1
                    ORDER BY COUNT(*) DESC;

                    -- 10. MONTHLY ORDER STATISTICS
                    SELECT
                        YEAR(o.OrderedDate) AS OrderYear,
                        MONTH(o.OrderedDate) AS OrderMonth,
                        COUNT(*) AS OrderCount,
                        SUM(o.TotalAmount) AS TotalOrderAmount,
                        AVG(o.TotalAmount) AS AverageOrderAmount
                    FROM [ordering].[Order] o
                    WHERE o.[Status] <> 'Cancelled'
                    GROUP BY YEAR(o.OrderedDate), MONTH(o.OrderedDate)
                    ORDER BY OrderYear, OrderMonth;

                    -- 11. PLUS: FORECAST CASH FLOW
                    SELECT
                        YEAR(i.DueDate) AS ForecastYear,
                        MONTH(i.DueDate) AS ForecastMonth,
                        COUNT(i.Id) AS UnpaidInstallmentsCount,
                        SUM(i.Amount) AS ExpectedCashFlow
                    FROM [ordering].[Installment] i
                    INNER JOIN [ordering].[PaymentInfo] p ON p.Id = i.PaymentInfoId
                    INNER JOIN [ordering].[Order] o ON o.Id = p.OrderId
                    WHERE o.[Status] <> 'Cancelled' AND i.IsPaid = 0
                    GROUP BY YEAR(i.DueDate), MONTH(i.DueDate)
                    ORDER BY ForecastYear, ForecastMonth;

                    -- 12. PLUS: OVERDUE COMPANIES SUMMARY
                    WITH CurrentPaymentRule AS (
                        SELECT TOP (1) COALESCE(GracePeriod, 0) AS GracePeriodDays
                        FROM [settings].[PaymentRules]
                        ORDER BY Id ASC
                    )
                    SELECT
                        o.Buyer_CompanyName AS CompanyName,
                        o.Buyer_Name AS ContactPerson,
                        o.Buyer_CompanyPhoneNumber AS PhoneNumber,
                        COUNT(i.Id) AS OverdueInstallmentsCount,
                        SUM(i.Amount) AS TotalOverdueDebt,
                        MAX(DATEDIFF(DAY, DATEADD(DAY, r.GracePeriodDays, i.DueDate), GETDATE())) AS MaxDaysOverdue
                    FROM [ordering].[Installment] i
                    INNER JOIN [ordering].[PaymentInfo] p ON p.Id = i.PaymentInfoId
                    INNER JOIN [ordering].[Order] o ON o.Id = p.OrderId
                    CROSS JOIN CurrentPaymentRule r
                    WHERE o.[Status] <> 'Cancelled' AND i.IsPaid = 0 AND GETDATE() > DATEADD(DAY, r.GracePeriodDays, i.DueDate)
                    GROUP BY o.Buyer_CompanyName, o.Buyer_Name, o.Buyer_CompanyPhoneNumber
                    ORDER BY TotalOverdueDebt DESC;
                END;
                GO
            ");

		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [ordering].[SP_GetOrderStatistics];");
		}
    }
}
