using BlazorWasmTodo.Extensions;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTodo.Services;

public class SyncServices
{
    private readonly FileSystemAccessService _fileSystemAccessService;
    private FileSystemFileHandle? _fileHandle;

    public SyncServices(FileSystemAccessService fileSystemAccessService)
    {
        _fileSystemAccessService = fileSystemAccessService;
    }

    public async Task InitFromOriginPrivateAsync(DbContext dbContext, CancellationToken cancellationToken = default)
        => await InitAsync(dbContext, await _fileSystemAccessService.GetOriginPrivateDirectoryAsync(), cancellationToken);

    public async Task InitAsync(DbContext dbContext, FileSystemHandle handle, CancellationToken cancellationToken = default)
    {
        var dataSource = dbContext.Database.GetDataSource();

        if (handle is FileSystemDirectoryHandle directoryHandle)
        {
            var fileOptions = new FileSystemGetFileOptions { Create = true };

            _fileHandle = await directoryHandle.GetFileHandleAsync(dataSource, fileOptions);

        }
        else if (handle is FileSystemFileHandle fileHandle)
        {
            _fileHandle = fileHandle;
        }
        else
        {
            throw new InvalidProgramException();
        }

        var file = await _fileHandle.GetFileAsync();

        var buffer = await file.ArrayBufferAsync();

        await dbContext.Database.CloseConnectionAsync();

        Console.WriteLine($"{dataSource} write {buffer.Length} bytes");

        await System.IO.File.WriteAllBytesAsync(dataSource, buffer, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await PersistenceAsync(dbContext, cancellationToken);
    }

    internal async Task PersistenceAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Start saving database");
        await dbContext.Database.CloseConnectionAsync();

        var dataSource = dbContext.Database.GetDataSource();
        var buffer = await System.IO.File.ReadAllBytesAsync(dataSource, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine($"{dataSource} read {buffer.Length} bytes");

        var options = new FileSystemCreateWritableOptions { KeepExistingData = false };
        var writer = await _fileHandle.CreateWritableAsync(options);

        await writer.WriteAsync(buffer, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await writer.CloseAsync();

        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine("Finish save database");
    }

    public async Task EnsureDeletedAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.CloseConnectionAsync();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await PersistenceAsync(dbContext, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
    }
}
