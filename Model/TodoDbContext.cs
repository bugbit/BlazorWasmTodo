using BlazorWasmTodo.Components.JS;
using BlazorWasmTodo.Components.Native;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
                .HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        Console.WriteLine("Start saving database");
        await JSFile.SyncDatabase(false);
        //CFile.syncDatabase(0);
        Console.WriteLine("Finish save database");

        return result;
    }
}
