using backoffice_worker.Abstractions;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Entities.Proccessing;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Abstraction;


namespace backoffice_worker.Services;

public class BillingTaskService : CronJobWorker
{
	private static bool _isInitialized = false;

	public BillingTaskService(IScheduleConfig<BillingTaskService> config, 
		IServiceProvider serviceProvider, ILogger<BillingTaskService> logger)
		: base(config.CronExpression, config.TimeZoneInfo, serviceProvider, logger)
	{ }

	public override async Task DoWork(IServiceScope scope, CancellationToken cancellationToken)
	{
		try
		{
			if (!_isInitialized)
			{
				_logger.LogInformation("Billing Task Running!!!");
				_isInitialized = true;
			}

			var billingTaskRepository =
				scope.ServiceProvider.GetRequiredService<IBilingTaskRepository>();

			var bilingTask = await billingTaskRepository.GetForProccessing();

			if (bilingTask.Count > 0)
			{
				var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
				var orderCosmosDb = scope.ServiceProvider.GetRequiredService<IOrderCosmosRepository>();

				_logger.LogInformation("Billing Task Start Processing!!!");
				foreach (var billing in bilingTask)
				{
					var order = await orderRepository.GetByIdAsync(billing.OrderId);
					bool success = false;
					if (order != null)
					{
						success = await SaveOnCosmosToBeginInvoicePDFCreationProccesing(order, billing, orderCosmosDb);
					}

					if (success && billing.IsFullPayment)
					{
						billing.MarkAsCompleted();
						_logger.LogInformation("BillingTask {BillingTaskId} completed for Order {OrderNumber}", billing.Id, order?.OrderNumber);
					}
				}

				await billingTaskRepository.UpdateBulkBillingTask(bilingTask);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
		}
	}

	private async Task<bool> SaveOnCosmosToBeginInvoicePDFCreationProccesing(Order? order, BillingTask billingTask, 
		IOrderCosmosRepository orderCosmosRepository)
	{
		if (order == null)
		{
			_logger.LogWarning("Order is null, cannot process BillingTask {BillingTaskId}", billingTask.Id);
			return false;
		}

		var installmentFromOrder = order.PaymentInfo?.Installments?.FirstOrDefault(x => x.Id == billingTask.InstallmentId);

		Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.Installment? installment = null;
		bool installmentIsCompleted = false;

		if (installmentFromOrder != null && !billingTask.IsFullPayment)
		{
			installment = new Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.Installment
			{
				DueDate = installmentFromOrder.DueDate,
				InstallmentNumber = billingTask.InstalmentNumber!.Value,
				Amount = installmentFromOrder.Amount,
				InstallmentCode = installmentFromOrder.InstallmentCode
			};

			_logger.LogInformation("Processing Installment {InstallmentCode} for BillingTask {BillingTaskId}",
			installment.InstallmentCode, billingTask.Id);
		}
		var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		var orderCosmos = new Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.Order
		{
			Id = order.PaymentInfo.WillBePaidInFull ? order.Id.ToString() : $"{order.Id.ToString()}_0{billingTask.InstalmentNumber}",
			OrderNumber = order.OrderNumber,
			Amount = order.Amount,
			TotalAmount = order.TotalAmount,
			Discount = order.Discount,
			OrderedDate = order.OrderedDate,
			Ttl = 7 * 24 * 60 * 60,
			Environment = env??"Development",
			Buyer = new Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.Buyer
			{
				Name = order.Buyer?.Name,
				PersonalEmail = order.Buyer?.PersonalEmail,
				CompanyAddress = order.Buyer?.CompanyAddress,
				CompanyCity = order.Buyer?.CompanyCity,
				CompanyEmail = order.Buyer?.CompanyEmail,
				CompanyName = order.Buyer?.CompanyName,
				CompanyLocation = order.Buyer?.CompanyLocation,
				CompanyPhoneNumber = order.Buyer?.CompanyPhoneNumber,
				JobTitle = order.Buyer?.JobTitle
			},
			PaymentInfo = new Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.PaymentInfo
			{
				FinalInstallmentDueDate = order.PaymentInfo?.FinalInstallmentDueDate,
				WillBePaidInFull = order.PaymentInfo?.WillBePaidInFull ?? false,
				PaymentDate = (DateTime)order.PaymentInfo?.PaymentDate,
				PaymentNumber = order.PaymentInfo?.PaymentNumber,
				AmountPerInstallment = order.PaymentInfo?.AmountPerInstallment,
				NumberOfInstallments = order.PaymentInfo?.NumberOfInstallments,
				Installments = installment
			},
			OrderDetails = order?.OrderDetails?.Select(d => new Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.OrderDetails
			{
				UnitOfMeasure = d.UnitOfMeasure,
				PricePerUnit = d.PricePerUnit,
				Quantity = d.Quantity,
				TotalPrice = d.TotalPrice,
				Product = new Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities.OrderedProduct
				{
					Name = d?.Product?.Name,
					Price = d.Product.Price,
					Units = d.Product?.Units,
					SeasonalFruits = d.Product.SeasonalFruits
				}
			}).ToList(),
			CreatedDate = order!.CreatedDate
		};

		var result = await orderCosmosRepository.AddOrder(orderCosmos);

		if (!result.Success)
		{
			billingTask.MarkAsFailed(result.ErrorMessage);

			_logger.LogError("Failed to add Order {OrderNumber} to CosmosDB. Error: {ErrorMessage}", order.OrderNumber, result.ErrorMessage);
			return false;
		}
		else
		{
			_logger.LogInformation("Order {OrderNumber} successfully saved to CosmosDB", order.OrderNumber);
		}

		if (!billingTask.IsFullPayment && !installmentIsCompleted && billingTask.InstalmentNumber.HasValue && 
			(billingTask.InstalmentNumber <= billingTask.NumberOfInstallments))
		{
			installmentIsCompleted = billingTask.SetInstallmentNumber();

			if (installmentIsCompleted)
			{
				_logger.LogInformation("BillingTask {BillingTaskId} completed for Order {OrderNumber}", billingTask.Id, order.OrderNumber);
				return false;
			}

			var nextInstallmentId = PrepareNextInstallmentId(order, billingTask.InstalmentNumber.Value);
			billingTask.SetInstallmentId(nextInstallmentId);

			_logger.LogInformation("BillingTask {BillingTaskId} set next InstallmentId {NextInstallmentId}, ScheduledFor {ScheduledFor}",
									billingTask.Id, nextInstallmentId, billingTask.ScheduledFor);
		}

		return true;
	}

	private int? PrepareNextInstallmentId(Order order, int installmentNumber)
	{
		var suffix = installmentNumber.ToString("D2");

		var nextInstallment = order.PaymentInfo?.Installments?
			.FirstOrDefault(x => x.InstallmentCode.EndsWith($"-{suffix}"));

		if (nextInstallment == null)
			throw new InvalidOperationException($"Next installment {suffix} not found");

		return nextInstallment.Id;
	}
}