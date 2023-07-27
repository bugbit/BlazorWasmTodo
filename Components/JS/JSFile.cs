using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BlazorWasmTodo.Components.JS;

[SupportedOSPlatform("browser")]
public partial class JSFile
{
    [JSImport("mountAndInitializeDb", "CallJSFile")]
    internal static partial Task MountAndInitializeDb();
    [JSImport("syncDatabase", "CallJSFile")]
    internal static partial Task SyncDatabase(bool populate);
}
