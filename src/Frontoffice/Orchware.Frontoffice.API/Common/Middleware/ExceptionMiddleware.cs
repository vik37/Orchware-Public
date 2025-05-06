using Newtonsoft.Json;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Models;
using System.Net;

namespace Orchware.Frontoffice.API.Common.Middleware;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleException(context, ex);
		}
	}

	private async Task HandleException(HttpContext context, Exception ex)
	{
		HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
		dynamic? problem = null;

		switch (ex)
		{
			case BadRequestException badRequestException:
				httpStatusCode = HttpStatusCode.BadRequest;
				problem = new CustomProblemDetails
				{
					Title = badRequestException.Message,
					Status = (int)httpStatusCode,
					Detail = badRequestException.InnerException?.Message,
					Type = nameof(BadRequestException),
					Errors = badRequestException.ValidationErrors!
				};
				break;
			case NotFoundException notFoundException:
				httpStatusCode = HttpStatusCode.NotFound;
				problem = new CustomProblemDetails
				{
					Title = notFoundException.Message,
					Status = (int)httpStatusCode,
					Detail = notFoundException.InnerException?.Message,
					Type = nameof(NotFoundException),
				};
				break;
			default:
				problem = new CustomProblemDetails
				{
					Title = ex.Message,
					Status = (int)httpStatusCode,
					Detail = ex.StackTrace,
					Type = nameof(HttpStatusCode.InternalServerError),
				};
				break;
		}

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)httpStatusCode;
		string jsonMessage = JsonConvert.SerializeObject(problem);

		await context.Response.WriteAsync(jsonMessage);
	}
}
