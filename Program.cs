using BlazorWasmTodo;
using BlazorWasmTodo.Components.JS;
using BlazorWasmTodo.Components.Native;
using BlazorWasmTodo.Model;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

string fileName = "/database/app.db";

builder.Services.AddDbContext<TodoDbContext>(
    options => options.UseSqlite($"Filename={fileName}"));

// sqllite and Persisting data with the WebAssembly File System API

Console.WriteLine("Before mountAndInitializeDbNative");
//CFile.mountAndInitializeDb();
Console.WriteLine("After mountAndInitializeDbNative");
await JSHost.ImportAsync("CallJSFile", "../js/file.js");
await JSFile.MountAndInitializeDb();
Console.WriteLine("After mountAndInitializeDbNative");
if (!File.Exists(fileName))
{
    File.Create(fileName).Close();
}
Console.WriteLine("After Create file");

var host = builder.Build();

var dbService = host.Services.GetRequiredService<TodoDbContext>();

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
