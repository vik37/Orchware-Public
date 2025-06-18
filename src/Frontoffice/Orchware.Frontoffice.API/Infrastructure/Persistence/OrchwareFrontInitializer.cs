using FileStorage;
using FileStorage.Enums;
using FileStorage.FileFormatServices;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Polly;
using Polly.Registry;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence;

public class OrchwareFrontInitializer
{
	private readonly OrchwareDbContext _context;
	private readonly  IFileServiceFactory _fileServiceFactory;
	private ResiliencePipeline _resiliencePipeline;
	private ILogger<OrchwareFrontInitializer> _logger;

	public OrchwareFrontInitializer(OrchwareDbContext context, IFileServiceFactory fileServiceFactory,
		ResiliencePipelineProvider<string> pipelineProvider, ILogger<OrchwareFrontInitializer> logger)
	{
		_context = context;
		_fileServiceFactory = fileServiceFactory;
		_resiliencePipeline = pipelineProvider.GetPipeline("orchware-frontoffice-pipeline");
		_logger = logger;
	}

	public async Task InitializeData(string filePath)
	{
		await _resiliencePipeline.ExecuteAsync(async token =>
		{
			try
			{
				await _context.Database.MigrateAsync();

				_logger.LogInformation("Started Database Initializer. File Path: {FILEPATH}", filePath);
				if (!_context.Product.Any() && File.Exists(filePath))
				{
					var fileService = _fileServiceFactory.Create<Product, CSVFormatFileService<Product>>(FileSource.Local, FileFormat.CSV);

					var products = await fileService.ReadFromFile(filePath);

					if (products != null)
					{
						await _context.Product.AddRangeAsync(products);
						await _context.SaveChangesAsync();
					}
				}

				_logger.LogInformation("Database Initializer Finished Successfully");
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "An error occurred while updating the database. Message: {MESSAGE}", ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurredduring database initialization. Message: {MESSAGE}", ex.Message);
			}
		});
	}
}
