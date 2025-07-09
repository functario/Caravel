using System.ComponentModel;
using System.Reflection;
using Caravel.Core;
using Caravel.Mermaid;
using ModelContextProtocol.Server;
using WebSite.Facade;

namespace Caravel.MCP.Server.Tools;

[McpServerToolType]
internal static class GotoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from Echo: {message}";

    [
        McpServerTool,
        Description(
            "Goto from the current opened page to another destination page"
                + " using the name of type of the Page in WebSite."
        )
    ]
    public static async Task<string> GotoAsync(WebSiteJourney journey, string destinationPage)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var webSiteAssembly = Assembly.GetAssembly(typeof(WebSiteJourney))!;
            var type = webSiteAssembly
                .GetTypes()
                ?.Where(x => x.Name == destinationPage)
                .FirstOrDefault()!;

            var method = typeof(WebSiteJourney)
                .GetMethod("GotoAsync", BindingFlags.Instance | BindingFlags.Public)!
                .MakeGenericMethod(type);

            // Invoke it and await the Task
            Waypoints waypoints = [];
            ExcludedNodes excludedNodes = [];
            var task = (Task)
                method.Invoke(journey, [waypoints, excludedNodes, CancellationToken.None])!;
            await task;

            // You can return something if needed; here we return the type name for illustration
            var sequence = await journey.ToMermaidSequenceDiagramMarkdownAsync();
            return $"The navigation sequence: '{sequence}'";
        }
        catch (Exception e)
        {
            return $"An exception occurs '{e}'";
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}
