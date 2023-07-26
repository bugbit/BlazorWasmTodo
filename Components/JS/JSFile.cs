using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BlazorWasmTodo.Components.JS;

[SupportedOSPlatform("browser")]
public partial class JSFile
{
    [JSImport("mountAndInitializeDb", "CallJSFile")]
    internal static partial void MountAndInitializeDb();
    [JSImport("syncDatabase", "CallJSFile")]
    internal static partial void SyncDatabase(bool populate);
}
