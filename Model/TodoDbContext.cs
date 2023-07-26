using BlazorWasmTodo.Components.JS;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTodo.Model;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions options) : base(options)
    {
    }

    public TodoDbContext()
    {
    }

    public DbSet<TodoItem> TodoItem { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        Console.WriteLine("Start saving database");
        JSFile.SyncDatabase(false);
        Console.WriteLine("Finish save database");

        return result;
    }
}
