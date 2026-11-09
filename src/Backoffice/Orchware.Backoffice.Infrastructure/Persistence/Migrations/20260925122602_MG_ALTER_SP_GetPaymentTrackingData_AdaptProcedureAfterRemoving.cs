using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_ALTER_SP_GetPaymentTrackingData_AdaptProcedureAfterRemoving_MaximumPaymentPeriodColumn_PaymentRuleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [ordering].[SP_GetPaymentTrackingData]
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        o.OrderNumber,
                        o.[Status],
                        o.Buyer_CompanyName,
                        o.Buyer_PersonalEmail,
                        o.OrderedDate,

                        pi.WillBePaidInFull,
                        pi.PaymentNumber,
                        pi.IsPaidInFull,
                        pi.PaidInFullDate,
                        pi.PaymentDate,

                        CAST(
                            CASE
                                WHEN pi.WillBePaidInFull = 1
                                     AND pi.InvoiceUrl IS NOT NULL
                                THEN 1
                                ELSE 0
                            END
                        AS BIT) AS HasInvoice,

                        i.InstallmentCode AS Code,
                        i.IsPaid,
                        i.PaidOnDate,
                        i.DueDate,

                        CAST(
                            CASE
                                WHEN i.InvoiceUrl IS NOT NULL
                                THEN 1
                                ELSE 0
                            END
                        AS BIT) AS HasInvoice

                    FROM [ordering].[PaymentInfo] AS pi
                    LEFT JOIN [ordering].[Installment] AS i
                        ON pi.Id = i.PaymentInfoId
                    RIGHT JOIN [ordering].[Order] AS o
                        ON o.Id = pi.OrderId

                    WHERE o.[Status] NOT IN ('Requested', 'Confirmed', 'Cancelled')
                      AND (
                            pi.PaidInFullDate <= (
                                SELECT TOP 1
                                    DATEADD(
                                        day,
                                        30 + ISNULL(GracePeriod, 0),
                                        pi.PaymentDate
                                    )
                                FROM [settings].[PaymentRules]
                            )
                            OR pi.PaidInFullDate IS NULL
                      )

                    ORDER BY
                        pi.PaymentDate,
                        pi.IsPaidInFull,
                        i.IsPaid DESC;
                END
            ");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [ordering].[SP_GetPaymentTrackingData];");
		}
    }
}
