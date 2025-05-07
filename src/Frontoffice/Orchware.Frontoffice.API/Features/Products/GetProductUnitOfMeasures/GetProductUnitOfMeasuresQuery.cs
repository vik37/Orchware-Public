using MediatR;

namespace Orchware.Frontoffice.API.Features.Products.GetProductUnitOfMeasures;

public record GetProductUnitOfMeasuresQuery : IRequest<List<ProductUnitOfMeasuresDto>>{}
