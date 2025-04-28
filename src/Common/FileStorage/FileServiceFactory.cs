using FileStorage.Enums;
using FileStorage.FileFormatServices;
using FileStorage.FileSourceServices;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage;

public class FileServiceFactory : IFileServiceFactory, IDisposable
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly Dictionary<Type, IDisposable> _activeInstances = new();
	private readonly Dictionary<(FileSource, FileFormat), Func<IServiceProvider, Type, Type, object>> _serviceFactories;

	public FileServiceFactory(IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;

		_serviceFactories = new Dictionary<(FileSource, FileFormat), Func<IServiceProvider, Type, Type, object>>
		{
			{ (FileSource.Local, FileFormat.CSV), (sp, t, u) => sp.GetRequiredService(typeof(LocalFileStorage<,>).MakeGenericType(t, typeof(CSVFormatFileService<>).MakeGenericType(t))) },
			{ (FileSource.Local, FileFormat.Excel), (sp, t, u) => sp.GetRequiredService(typeof(LocalFileStorage<,>).MakeGenericType(t, typeof(ExcelFormatFileService<>).MakeGenericType(t))) },
			{ (FileSource.AzureBlobs, FileFormat.CSV), (sp, t, u) => sp.GetRequiredService(typeof(AzureFileStorage<,>).MakeGenericType(t, typeof(CSVFormatFileService<>).MakeGenericType(t))) },
			{ (FileSource.AzureBlobs, FileFormat.Excel), (sp, t, u) => sp.GetRequiredService(typeof(AzureFileStorage<,>).MakeGenericType(t, typeof(ExcelFormatFileService<>).MakeGenericType(t))) }
		};
	}

	public IFileService<T,U> Create<T,U>(FileSource fileSource, FileFormat fileFormat)
		where T : class, new()
		where U : IFileFormat<T>
	{
		var scope = _serviceScopeFactory.CreateScope();
		var provider = scope.ServiceProvider;

		if (!_serviceFactories.TryGetValue((fileSource, fileFormat), out var factory))
		{
			scope.Dispose();
			throw new NotSupportedException($"Unsupported source/format: {fileSource} + {fileFormat}");
		}

		var instance = (IFileService<T, U>)factory(provider, typeof(T), typeof(U));

		if (instance is IDisposable disposableInstance)
		{
			var wrapper = new DisposableWrapper(disposableInstance, scope);
			_activeInstances.Add(typeof(T), wrapper);
		}
		else
		{
			_activeInstances.Add(typeof(T), scope);
		}

		return instance;
	}

	public void Dispose()
	{
		foreach (var instance in _activeInstances.Values)
			instance.Dispose();

		_activeInstances.Clear();
	}

	private class DisposableWrapper : IDisposable
	{
		private readonly IDisposable _disposable1;
		private readonly IDisposable _disposable2;

		public DisposableWrapper(IDisposable disposable1, IDisposable disposable2 = null)
		{
			_disposable1 = disposable1;
			_disposable2 = disposable2;
		}

		public void Dispose()
		{
			_disposable2?.Dispose();
			_disposable1.Dispose();
		}
	}
}