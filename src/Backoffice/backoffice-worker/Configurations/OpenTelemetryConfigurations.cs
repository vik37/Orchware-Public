using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace backoffice_worker.Configurations;

public static class OpenTelemetryConfigurations
{
	public static IServiceCollection AddOpenTelemetryRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		var otelSettings = configuration.GetSection("OpenTelemetry:OtlpExporter").Get<OtelSettings>();

		var resourceBuilder = ResourceBuilder.CreateDefault().AddService("orchware-backoffice-worker");

		services.AddOpenTelemetry()
			.WithTracing(tracingProvider =>
			{
				tracingProvider.SetResourceBuilder(resourceBuilder)
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddOtlpExporter(otlp =>
				{
					otlp.Endpoint = new Uri(otelSettings!.Endpoint);
					otlp.Protocol = otelSettings!.Protocol.ToLower() == "grpc" ?
														OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
				});
			})
			.WithMetrics(matricsProvider =>
			{
				matricsProvider.SetResourceBuilder(resourceBuilder)
								.AddHttpClientInstrumentation()
								.AddRuntimeInstrumentation()
								.AddOtlpExporter(otpl =>
								{
									otpl.Endpoint = new Uri(otelSettings!.Endpoint);
									otpl.Protocol = otelSettings.Protocol.ToLower() == "grpc" ?
														OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
								});
			});

		return services;
	}
}
