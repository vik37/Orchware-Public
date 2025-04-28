using FileStorage;
using FileStorage.Enums;
using FileStorage.FileFormatServices;
using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Enums;

namespace Orchware.Backoffice.Infrastructure.Persistence;

public class OrchwareBackInitializer
{
	private readonly OrchwareBackofficeDbContext _context;
	private readonly IFileServiceFactory _fileServiceFactory;

	public OrchwareBackInitializer(OrchwareBackofficeDbContext context, IFileServiceFactory fileServiceFactory)
	{
		_context = context;
		_fileServiceFactory = fileServiceFactory;
	}

	public async Task InitializeData()
	{
		await _context.Database.MigrateAsync();

		if (!_context.Product.Any() && !_context.Shelf.Any())
		{
			try
			{
				var shelvesTask = Task.Run(() => SafeGetDataFromFile<Shelf>($"{nameof(Shelf)}.csv"));
				var productsTask = Task.Run(() => SafeGetDataFromFile<Product>($"{nameof(Product)}.csv"));

				await Task.WhenAll(shelvesTask, productsTask);

				var shelves = shelvesTask.Result;
				var products = productsTask.Result;

				var winterShelves = shelves.Where(s => s.SeasonalFruits == SeasonalFruits.Winter).ToList();
				var springShelves = shelves.Where(s => s.SeasonalFruits == SeasonalFruits.Spring).ToList();
				var summerShelves = shelves.Where(s => s.SeasonalFruits == SeasonalFruits.Summer).ToList();

				AssignProductsToShelves(winterShelves, products.Take(20).ToList());

				AssignProductsToShelves(springShelves, products.Skip(20).Take(20).ToList());

				AssignProductsToShelves(summerShelves, products.Skip(41).ToList());

				_context.Shelf.AddRange(shelves);
				await _context.SaveChangesAsync();
			}
			catch(FileNotFoundException ex)
			{
				Console.WriteLine($"The file {ex.FileName} was not found in the path {Directory.GetCurrentDirectory()}");
			}
			catch (DbUpdateException ex)
			{
				Console.WriteLine("An error occurred while updating the database.", ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An unexpected error occurred.", ex);
			}
		}
	}

	private List<T> SafeGetDataFromFile<T>(string filename) where T : class, new()
	{
		try
		{
			return GetDataFromFile<T>(filename).ToList();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error loading {filename}: {ex.Message}");
			return new List<T>();
		}
	}


	private IEnumerable<T> GetDataFromFile<T>(string filename) where T : class, new()
	{
		var filePath = GetSeederFilePath(filename);
		var fileService = _fileServiceFactory.Create<T,CSVFormatFileService<T>>(FileSource.Local, FileFormat.CSV);
		return fileService.ReadFromFile(filePath).Result;
	}

	private string GetSeederFilePath(string fileName)
	{
		var basePath = Directory.GetCurrentDirectory().Replace("Orchware.Backoffice.API", "Orchware.Backoffice.Infrastructure");
		var file = Path.Combine(basePath, "Persistence", "SeedFiles", fileName);

		if (!File.Exists(file))
			throw new FileNotFoundException($"The file {fileName} was not found in the path {basePath}");

		return file;
	}

	private void AssignProductsToShelves(List<Shelf> shelves, List<Product> products)
	{
		int productIndex = 0;

		foreach (var shelf in shelves)
		{
			int capacity = 4;
			shelf.Products = products.Skip(productIndex).Take(capacity).ToList();

			foreach (var product in shelf.Products)
				product.Shelf = shelf;

			productIndex += capacity;

			if (productIndex >= products.Count)
				break;
		}
	}
}
