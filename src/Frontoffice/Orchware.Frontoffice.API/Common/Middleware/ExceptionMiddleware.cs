using Newtonsoft.Json;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Common.Models;
using System.Net;

namespace Orchware.Frontoffice.API.Common.Middleware;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionMiddleware> _logger;

	public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
	{
		_next = next;
		_logger = logger;
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
				_logger.LogWarning("HTTP Status: {HTTP_STATUS}, Message: {MESSAGE}", httpStatusCode, badRequestException.Message);
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
				_logger.LogWarning("HTTP Status: {HTTP_STATUS}, Message: {MESSAGE}", httpStatusCode, notFoundException.Message);
				break;
			default:
				problem = new CustomProblemDetails
				{
					Title = "Oops! Something went wrong.",
					Status = (int)httpStatusCode,
					Detail = "An error occurred while processing your request. Please try refreshing the page or" +
					" try again later.",
					Type = nameof(HttpStatusCode.InternalServerError),
				};
				_logger.LogError(ex, "Server Issue: {Message}. Inner Exception: {InnerMessage}", ex.Message, ex.InnerException?.Message);
				break;
		}

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)httpStatusCode;
		string jsonMessage = JsonConvert.SerializeObject(problem);

		await context.Response.WriteAsync(jsonMessage);
	}
}
