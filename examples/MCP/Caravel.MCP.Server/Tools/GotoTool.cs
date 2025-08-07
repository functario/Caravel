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
        Description("Goto from the current opened WebSite page to another destination page.")
    ]
    public static async Task<string> GotoAsync(WebSiteJourney journey, string destinationPage)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var destinationType = GetDestinationType(destinationPage);

            Waypoints waypoints = [];
            ExcludedNodes excludedNodes = [];

            await journey
                .GotoAsync(destinationType, waypoints, excludedNodes, CancellationToken.None)
                .ConfigureAwait(false);

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

    private static Type GetDestinationType(string destinationPage)
    {
        var webSiteAssembly = Assembly.GetAssembly(typeof(WebSiteJourney))!;
        return webSiteAssembly
            .GetTypes()
            ?.Where(x =>
                x.Name.Equals(destinationPage.Trim([' ', '.']), StringComparison.OrdinalIgnoreCase)
            )
            .FirstOrDefault()!;
    }
}
