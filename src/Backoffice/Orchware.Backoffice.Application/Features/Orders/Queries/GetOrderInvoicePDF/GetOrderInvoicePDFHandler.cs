using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;
using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices.Enum;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Backoffice.Application.Features.Shared.Responses;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.GetOrderInvoicePDF;

public record GetOrderInvoicePDFQuery(string PaymentNumber, string? InstallmentNumber = null) : IRequest<FileResponse?>;

public class GetOrderInvoicePDFHandler : IRequestHandler<GetOrderInvoicePDFQuery, FileResponse?>
{
	private readonly string _connectionString;
	private readonly ISqlQueryBuilder _sqlQueryBuilder;
	private string Schema = $"[{DomainSchema.Order}]";
	private readonly IFileStorageService _fileService;

	public GetOrderInvoicePDFHandler(IOptions<DatabaseConnectionOption> dbConnection, ISqlQueryBuilder sqlQueryBuilder,
		IFileStorageFactory fileFactory)
	{
		_connectionString = dbConnection.Value.MSSQLDbConnection;
		_sqlQueryBuilder = sqlQueryBuilder;
		_fileService = fileFactory.Initialize(FileType.Azure);
	}

	public async Task<FileResponse?> Handle(GetOrderInvoicePDFQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return null;

		var parameters = new DynamicParameters();
		var builder = CreateAppropriateDatabase(out parameters, request.PaymentNumber, request.InstallmentNumber);

		using var connection = new SqlConnection(_connectionString);
		var result = await connection.QueryFirstOrDefaultAsync<dynamic>(builder, parameters);

		if (result == null || string.IsNullOrEmpty(result!.InvoiceUrl))
			throw new FileNotFoundException("Missing Invoice.");

		string invoiceUrl = result!.InvoiceUrl;
		var storageService = await _fileService.DownloadAsync(invoiceUrl);

		if (storageService == null)
			throw new FileNotFoundException("Missing Invoice.");

		var uri = new Uri(invoiceUrl);
		string fileName = Path.GetFileName(uri.LocalPath);

		return new FileResponse
		{
			FileStream = storageService,
			FileName = string.IsNullOrEmpty(fileName) ? "invoice.pdf" : fileName
		};
	}

	private string CreateAppropriateDatabase(out DynamicParameters? parameters, string paymentNumber, string? installmentNumber = null)
	{
		parameters = new DynamicParameters();

		if (string.IsNullOrEmpty(installmentNumber))
		{
			parameters.Add("@PaymentNumber", paymentNumber);

			return _sqlQueryBuilder.Select("InvoiceUrl")
										.From($"{Schema}.[PaymentInfo]")
										.Where("PaymentNumber = @PaymentNumber")
										.Build();
		}

		string fullInvoiceCode = $"{paymentNumber}-{installmentNumber}";

		parameters.Add("@PaymentNumber", fullInvoiceCode);

		return _sqlQueryBuilder.Select("InvoiceUrl")
									.From($"{Schema}.[Installment]")
									.Where("InstallmentCode = @PaymentNumber")
									.Build();

	}
}
