using Microsoft.EntityFrameworkCore;

namespace BlazorWasmTodo.Model;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions options) : base(options)
    {
    }

    protected TodoDbContext()
    {
    }

    public DbSet<TodoItem> TodoItem { get; set; }
}
