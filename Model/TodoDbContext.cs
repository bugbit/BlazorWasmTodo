using BlazorWasmTodo.Services;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTodo.Model;

public class TodoDbContext : DbContext
{
    private readonly SyncServices _syncServices;

    public TodoDbContext(SyncServices syncServices, DbContextOptions options) : base(options)
    {
        _syncServices = syncServices;
    }

    public TodoDbContext(SyncServices syncServices)
    {
        _syncServices = syncServices;
    }

    public DbSet<TodoItem> TodoItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
                .HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);
    }

    // sqllite and Persisting data with the WebAssembly File System API
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        // sqllite and Persisting data with the WebAssembly File System API
        await _syncServices.PersistenceAsync(this, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        return result;
    }
}
