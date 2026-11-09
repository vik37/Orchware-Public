using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class MG_UpdateSP_GetOrderDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
		migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [ordering].[SP_GetOrderDetails]
                (
                    @OrderId UNIQUEIDENTIFIER = NULL,
					@OrderNumber NVARCHAR(100) = NULL,
                    @UserRole NVARCHAR(50)
                )
                AS
                BEGIN
                    SET NOCOUNT ON;

					DECLARE @InternalOrderId UNIQUEIDENTIFIER;

					IF @OrderId IS NOT NULL
					BEGIN
						SET @InternalOrderId = @OrderId;
					END
					ELSE IF @OrderNumber IS NOT NULL
					BEGIN
						SELECT TOP 1 @InternalOrderId = Id 
						FROM [Ordering].[Order] 
						WHERE OrderNumber = @OrderNumber;
					END

					IF @InternalOrderId IS NULL RETURN;

                    DECLARE @Now DATETIME = GETDATE();

                    /* =========================================================
                       1. ORDER HEADER
                    ========================================================== */

                    SELECT
                        o.OrderNumber,
                        o.OrderedDate,

                        CASE
                            WHEN @UserRole = 'Manager'
                            THEN o.Discount
                        END AS Discount,

                        CASE
                            WHEN @UserRole = 'Manager'
                            THEN o.Amount
                        END AS Amount,

                        CASE
                            WHEN @UserRole = 'Manager'
                            THEN o.TotalAmount
                        END AS TotalAmount,
                        CASE
							WHEN @UserRole = 'Manager'
								 AND o.[Status] = 'Cancelled'
							THEN o.CancellationReason
						END AS CancellationReason,

						CASE
							WHEN @UserRole = 'Manager'
								 AND o.[Status] = 'Cancelled'
							THEN o.CancelledBy
						END AS CancelledBy,
                        o.[Status],
                        o.Buyer_Name AS PersonPlacedOrderName,
                        o.Buyer_PersonalEmail AS PersonPlacedOrderEmailContact,
                        o.Buyer_CompanyName AS CompanyName,
                        o.Buyer_CompanyAddress AS CompanyAddress,
                        o.Buyer_CompanyEmail AS CompanyEmail,
                        o.Buyer_CompanyCity AS CompanyCity,
                        o.Buyer_CompanyLocation AS CompanyLocation,
                        o.Buyer_CompanyPhoneNumber AS CompanyPhoneNumber,
                        ISNULL(
                        (
                            SELECT
				                SUM(
                            CASE LOWER(od.UnitOfMeasure)
                                WHEN 't' THEN od.Quantity * 1000.0
                                WHEN 'g' THEN od.Quantity * 0.001
                                WHEN 'kg' THEN od.Quantity * 1.0
                                ELSE 0.0
                            END
                        )
                            FROM [Ordering].[OrderDetails] od
                            WHERE od.OrderId = o.Id
                        ), 0) AS TotalQuantityInKg

                    FROM [Ordering].[Order] AS o
                    WHERE o.Id = @InternalOrderId;

                    /* =========================================================
                       2. ORDER ITEMS
                    ========================================================== */

                    SELECT
                        od.ShelfCode,
		                p.[Name] as ProductName,
		                p.[Image] as ProductImage,
		                CASE
			                WHEN @UserRole = 'Manager'
			                THEN p.SeasonalFruits
		                END AS ProductSeasonalFruits,

		                CASE
			                WHEN @UserRole = 'Manager'
			                THEN p.Price
		                END AS ProductPrice,

		                CASE
			                WHEN @UserRole = 'Manager'
			                THEN JSON_VALUE(od.Product, '$.Units')
		                END AS [ProductUnits],
                        od.Quantity,
                        od.UnitOfMeasure,
                        CASE
                            WHEN @UserRole = 'Manager'
                            THEN od.PricePerUnit
                        END AS PricePerUnit,

                        CASE
                            WHEN @UserRole = 'Manager'
                            THEN od.TotalPrice
                        END AS TotalPrice
                    FROM [Ordering].[OrderDetails] od
	                CROSS APPLY OPENJSON(od.Product)
	                WITH
	                (
		                [Name] NVARCHAR(200) '$.Name',
		                [Image] NVARCHAR(500) '$.Image',
		                SeasonalFruits NVARCHAR(100) '$.SeasonalFruits',
		                Price DECIMAL(18,2) '$.Price',
		                Units NVARCHAR(50) '$.Units'
	                ) p
                    WHERE od.OrderId = @InternalOrderId;

                    /* =========================================================
                       3. PAYMENT INFO (MANAGER ONLY)
                    ========================================================== */

                    IF @UserRole = 'Manager'
                    BEGIN

                        SELECT
                            pi.OrderId,
                            pi.PaymentNumber,
                            pi.WillBePaidInFull,
                            o.TotalAmount,
                            CASE 
								WHEN pi.WillBePaidInFull = 1 THEN pi.PaymentDate 
								ELSE pi.FinalInstallmentDueDate 
							END AS Deadline,
                            pi.IsPaidInFull AS IsFullySettled,
                            ISNULL(
                            (
                                SELECT SUM(i.Amount)
                                FROM [Ordering].[Installment] i
                                WHERE i.PaymentInfoId = pi.Id
                                AND i.IsPaid = 0
                            ), 0) AS TotalRemainingAmount,
                            (
                                SELECT MIN(i.DueDate)
                                FROM [Ordering].[Installment] i
                                WHERE i.PaymentInfoId = pi.Id
                                AND i.IsPaid = 0
                            ) AS NextIncomingPaymentDate,
                            CASE
                                WHEN EXISTS
                                (
                                    SELECT 1
                                    FROM [Ordering].[Installment] i
                                    WHERE i.PaymentInfoId = pi.Id
                                    AND i.IsPaid = 0
                                    AND i.DueDate < @Now
                                )
                                THEN CAST(1 AS BIT)
                                ELSE CAST(0 AS BIT)
                            END AS HasOverdueInstallments,
                            ISNULL(
                            (
                                SELECT SUM(i.Amount)
                                FROM [Ordering].[Installment] i
                                WHERE i.PaymentInfoId = pi.Id
                                AND i.IsPaid = 0
                                AND i.DueDate < @Now
                            ), 0) AS OverdueBalance

                        FROM [Ordering].[PaymentInfo] pi
                        INNER JOIN [Ordering].[Order] o
                            ON o.Id = pi.OrderId
                        WHERE pi.OrderId = @InternalOrderId;

                    END
                    ELSE
                    BEGIN

                        SELECT TOP 0
                            CAST(NULL AS UNIQUEIDENTIFIER) AS OrderId,
                            CAST(NULL AS NVARCHAR(50)) AS PaymentNumber,
                            CAST(NULL AS BIT) AS WillBePaidInFull,
                            CAST(NULL AS DECIMAL(18,2)) AS TotalAmount,
                            CAST(NULL AS DATETIME) AS Deadline,
                            CAST(NULL AS BIT) AS IsFullySettled,
                            CAST(NULL AS DECIMAL(18,2)) AS TotalRemainingAmount,
                            CAST(NULL AS DATETIME) AS NextIncomingPaymentDate,
                            CAST(NULL AS BIT) AS HasOverdueInstallments,
                            CAST(NULL AS DECIMAL(18,2)) AS OverdueBalance;
                    END
                END
                GO
            ");
	}

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
		migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [ordering].[SP_GetOrderDetails];");
	}
}
