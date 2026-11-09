using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orchware.Notification.API.Models.Requests;

namespace Orchware.Notification.API.Filters;

public class ValidateNotificationRequestAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var request = context.ActionArguments.Values.OfType<PagedRequest>().FirstOrDefault();

		if (request == null) return;

		if (request.Take <= 0) request.Take = 10;
		if (request.Skip < 0) request.Skip = 0;

		if (request.Filter?.Filter != null && request.Filter.Filter.Any())
		{
			DateTime? fromDate = null;
			DateTime? toDate = null;

			foreach (var filter in request.Filter.Filter)
			{
				if (filter.Key == "FromDate")
				{
					if (DateTime.TryParse(filter.Value, out var parsedFrom))
						fromDate = parsedFrom;
					else
					{
						context.Result = new BadRequestObjectResult("Invalid FromDate format.");
						return;
					}
				}

				if (filter.Key == "ToDate")
				{
					if (DateTime.TryParse(filter.Value, out var parsedTo))
						toDate = parsedTo;
					else
					{
						context.Result = new BadRequestObjectResult("Invalid ToDate format.");
						return;
					}
				}
			}

			if (fromDate.HasValue && toDate.HasValue && toDate.Value < fromDate.Value)
			{
				context.Result = new BadRequestObjectResult("ToDate cannot be earlier than FromDate.");
				return;
			}
		}

		base.OnActionExecuting(context);
	}
}
