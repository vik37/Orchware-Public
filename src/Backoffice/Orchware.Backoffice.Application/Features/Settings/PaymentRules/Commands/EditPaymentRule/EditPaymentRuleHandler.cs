using MediatR;
using Orchware.Backoffice.Application.Features.Settings.PaymentRules.Shared.Dtos;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Domain.Entities.Settings;

namespace Orchware.Backoffice.Application.Features.Settings.PaymentRules.Commands.EditPaymentRule;

public record EditPaymentRuleCommand(PaymentRuleDto Model) : IRequest<Unit>;

public class EditPaymentRuleHandler : IRequestHandler<EditPaymentRuleCommand, Unit>
{
	private readonly IPaymentRulesRepository _paymentRulesRepository;

	public EditPaymentRuleHandler(IPaymentRulesRepository paymentRulesRepository)
	{
		_paymentRulesRepository = paymentRulesRepository;
	}

	public async Task<Unit> Handle(EditPaymentRuleCommand command, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return Unit.Value;

		var paymentRule = await _paymentRulesRepository.GetByIdAsync(command.Model.Id);

		if(paymentRule is null)
			throw new NotFoundException("Payment Rules does not have any data");


		paymentRule.Update(command.Model.MaxAllowedInstallments, 
							command.Model.GracePeriod, 
							command.Model.AllowDiscountModification);

		if (command.Model.DiscountDescription is not null && command.Model.AllowDiscountModification)
			paymentRule.UpdateDiscount(command.Model.DiscountDescription.MinimumOrderAmountForDiscount,
										command.Model.DiscountDescription.OrderAmountDiscountPercentage,
										command.Model.DiscountDescription.PayImmediatelyDiscountPercentage);

		await _paymentRulesRepository.UpdateAsync(paymentRule);
		await _paymentRulesRepository.UnitOfWork.SaveChangesAsync();

		return Unit.Value;
	}
}
