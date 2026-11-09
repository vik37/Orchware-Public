using MediatR;

namespace Orchware.Frontoffice.API.Features.Payments.PaymentRuleDescription;

public record PaymentRuleDescriptionQuery : IRequest<PaymentRuleDescriptionResponse>;
