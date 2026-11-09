namespace Orchware.Backoffice.Application.Features.Orders.Queries.GetOrderStatistics;

public class GetOrderStatisticsResponse
{
	public OverallStatsDto Overall { get; set; } = new();
	public IEnumerable<CompanyStatsDto> Companies { get; set; } = new List<CompanyStatsDto>();
	public IEnumerable<BuyerCompanyStatsDto> Buyers { get; set; } = new List<BuyerCompanyStatsDto>();
	public PaymentStatsDto Payments { get; set; } = new();
	public IEnumerable<CancellationStatsDto> Cancellations { get; set; } = new List<CancellationStatsDto>();
	public ProductStatsDto TopProductByQuantity { get; set; } = new();
	public ProductStatsDto TopProductByOrders { get; set; } = new();
	public UniqueCountsStatsDto UniqueCounts { get; set; } = new();
	public IEnumerable<BuyerCompanyStatsDto> RepeatBuyers { get; set; } = new List<BuyerCompanyStatsDto>();
	public IEnumerable<MonthlyStatsDto> Monthly { get; set; } = new List<MonthlyStatsDto>();
	public IEnumerable<CashFlowForecastDto> CashFlow { get; set; } = new List<CashFlowForecastDto>();
	public IEnumerable<OverdueCompanyStatsDto> OverdueCompanies { get; set; } = new List<OverdueCompanyStatsDto>();
}

public class OverallStatsDto
{
	public int TotalOrders { get; set; }
	public decimal TotalOrderAmount { get; set; }
	public decimal AverageOrderAmount { get; set; }
	public string? FirstOrderNumber { get; set; }
	public DateTime? FirstOrderDate { get; set; }
	public string? LatestOrderNumber { get; set; }
	public DateTime? LatestOrderDate { get; set; }
}

public class CompanyStatsDto
{
	public string CompanyName { get; set; } = string.Empty;
	public int OrderCount { get; set; }
	public decimal TotalOrderAmount { get; set; }
	public decimal AverageOrderAmount { get; set; }
}

public class BuyerCompanyStatsDto
{
	public string BuyerName { get; set; } = string.Empty;
	public string PersonalEmail { get; set; } = string.Empty;
	public string CompanyName { get; set; } = string.Empty;
	public int OrderCount { get; set; }
	public decimal TotalOrderAmount { get; set; }
	public decimal AverageOrderAmount { get; set; }
}

public class PaymentStatsDto
{
	public int TotalOrders { get; set; }
	public int FullyPaidOrders { get; set; }
	public decimal FullyPaidOrderValue { get; set; }
	public decimal TotalActuallyPaid { get; set; }
	public decimal OutstandingBalance { get; set; }
	public int PayInFullOrders { get; set; }
	public int InstallmentOrders { get; set; }
}

public class CancellationStatsDto
{
	public string CancelledBy { get; set; } = "None";
	public int CancelledOrderCount { get; set; }
	public decimal CancelledOrderValue { get; set; }
}

public class ProductStatsDto
{
	public string ProductName { get; set; } = string.Empty;
	public string? ProductImage { get; set; }
	public int OrderCount { get; set; }
	public decimal TotalQuantityKg { get; set; }
	public decimal TotalSalesAmount { get; set; }
}

public class UniqueCountsStatsDto
{
	public int UniqueBuyers { get; set; }
	public int UniqueCompanies { get; set; }
}

public class MonthlyStatsDto
{
	public int OrderYear { get; set; }
	public int OrderMonth { get; set; }
	public int OrderCount { get; set; }
	public decimal TotalOrderAmount { get; set; }
	public decimal AverageOrderAmount { get; set; }
}

public class CashFlowForecastDto
{
	public int ForecastYear { get; set; }
	public int ForecastMonth { get; set; }
	public int UnpaidInstallmentsCount { get; set; }
	public decimal ExpectedCashFlow { get; set; }
}

public class OverdueCompanyStatsDto
{
	public string CompanyName { get; set; } = string.Empty;
	public string ContactPerson { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
	public int OverdueInstallmentsCount { get; set; }
	public decimal TotalOverdueDebt { get; set; }
	public int MaxDaysOverdue { get; set; }
}
