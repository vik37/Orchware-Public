using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_SP_GetPaymentTrackingData : Migration
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
                                pi.WillBePaidInFull, 
                                pi.PaymentNumber, 
                                pi.IsPaidInFull, 
                                pi.PaidInFullDate, 
                                pi.PaymentDate, 
                                i.InstallmentCode as Code, 
                                i.IsPaid, 
                                i.PaidOnDate,
		                        i.DueDate
                            FROM [ordering].[PaymentInfo] AS pi
                            LEFT JOIN [ordering].[Installment] AS i ON pi.Id = i.PaymentInfoId
                            RIGHT JOIN [ordering].[Order] AS o ON o.Id = pi.OrderId
                            WHERE o.[Status] NOT IN ('Requested','Confirmed','Cancelled', 'Approved') 
                              AND (
                                    pi.PaidInFullDate <= (
                                        SELECT TOP 1 DATEADD(day, ISNULL(MaximumPaymentPeriod, 0) + ISNULL(GracePeriod, 0), pi.PaymentDate) 
                                        FROM [settings].[PaymentRules]
                                    )
                                    OR pi.PaidInFullDate IS NULL
                              )
                            ORDER BY pi.PaymentDate, pi.IsPaidInFull, i.IsPaid DESC;
                        END
                        GO

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [ordering].[SP_GetPaymentTrackingData];");
		}
    }
}
