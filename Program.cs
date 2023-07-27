using BlazorWasmTodo;
using BlazorWasmTodo.Model;
using BlazorWasmTodo.Services;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFileSystemAccessService();

// sqllite and Persisting data with the WebAssembly File System API
builder.Services.AddScoped<SyncServices>();

builder.Services.AddDbContext<TodoDbContext>(
    options => options.UseSqlite("Data Source=todo.sqlite3"));

var host = builder.Build();

var dbService = host.Services.GetRequiredService<TodoDbContext>();

// sqllite and Persisting data with the WebAssembly File System API
var syncService = host.Services.GetRequiredService<SyncServices>();

await syncService.InitFromOriginPrivateAsync(dbService);

await dbService.Database.EnsureCreatedAsync();

await host.RunAsync();

public partial class Program
{
    /// <summary>
    /// https://github.com/dotnet/efcore/issues/26860
    /// https://github.com/dotnet/aspnetcore/issues/39825
    /// FIXME: This is required for EF Core 6.0 as it is not compatible with trimming.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private static Type _keepDateOnly = typeof(DateOnly);
}
