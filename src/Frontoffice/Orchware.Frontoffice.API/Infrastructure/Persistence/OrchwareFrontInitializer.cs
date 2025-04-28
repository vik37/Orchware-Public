using FileStorage;
using FileStorage.Enums;
using FileStorage.FileFormatServices;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence;

public class OrchwareFrontInitializer
{
	private readonly OrchwareDbContext _context;
	private readonly  IFileServiceFactory _fileServiceFactory;

	public OrchwareFrontInitializer(OrchwareDbContext context, IFileServiceFactory fileServiceFactory)
	{
		_context = context;
		_fileServiceFactory = fileServiceFactory;
	}

	public async Task InitializeData(string filePath)
	{
		await _context.Database.MigrateAsync();

		if (!_context.Product.Any() && File.Exists(filePath))
		{
			var fileService = _fileServiceFactory.Create<Product,CSVFormatFileService<Product>>(FileSource.Local, FileFormat.CSV);

			var products = await fileService.ReadFromFile(filePath);

			if (products != null)
			{
				await _context.Product.AddRangeAsync(products);
				await _context.SaveChangesAsync();
			}
		}
	}
}
