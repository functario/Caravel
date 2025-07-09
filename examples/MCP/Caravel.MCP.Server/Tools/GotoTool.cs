using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using ModelContextProtocol.Server;
using WebSite.Facade;

namespace Caravel.MCP.Server.Tools;

[McpServerToolType]
internal static class GotoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    [
        McpServerTool,
        Description(
            "Goto from the current opened page to another destination page"
                + " using the name of type of the Page in WebSite."
        )
    ]
    public static async Task<string> GotoAsync(WebSiteJourney journey, string destinationPage)
    {
        var webSiteAssembly = Assembly.GetAssembly(typeof(WebSiteJourney))!;
        var type = webSiteAssembly.GetType(destinationPage)!;
        Debug.WriteLine(type);

        var method = typeof(WebSiteJourney)
            .GetMethod("GotoAsync", BindingFlags.Instance | BindingFlags.Public)!
            .MakeGenericMethod(type);

        Debug.WriteLine(method);

        // Invoke it and await the Task
        var task = (Task)method.Invoke(journey, null)!;
        await task;

        // You can return something if needed; here we return the type name for illustration
        return $"Navigated to {destinationPage}";
    }
}
