using Orchware.Backoffice.Domain.Enums;
using Orchware.Backoffice.Domain.Primitives;
using Utilities.Converters;

namespace Orchware.Backoffice.Domain.Entities.Inventory;

public sealed class Shelf : AggregateRoot<int>
{
	public string Code { get; set; } = string.Empty;
	public SeasonalFruits SeasonalFruits { get; set; }
	public IList<Product> Products { get; set; } = new List<Product>();

	public void DecreaseStock(int productId, decimal quantity, string unitOfMeagure)
	{
		var product = this.Products.FirstOrDefault(x => x.Id == productId);

		if(product is not null)
		{
			product.AvailableQuantity -= (float)UnitConverter.Convert(quantity, unitOfMeagure, product.Units.GetDisplayNameToString());
		}
	}
}
