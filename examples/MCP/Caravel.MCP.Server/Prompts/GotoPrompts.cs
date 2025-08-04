using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using WebSite.Facade.POMs.Abstractions;

namespace Caravel.MCP.Server.Prompts;

[McpServerPromptType]
internal static class GotoPrompts
{
    [McpServerPrompt, Description("List the name of the pages that can be opened.")]
    public static ChatMessage ListPages()
    {
        var types = JsonSerializer.Serialize(GetTypeAssignableFrom<IPOM>());
        var content = string.Format(CultureInfo.InvariantCulture, types);
        return new(ChatRole.User, content);
    }

    public static ICollection<Type> GetTypeAssignableFrom<T>()
    {
        var interfaceType = typeof(T);
        var assembly = Assembly.GetExecutingAssembly();

        return
        [
            .. assembly
                .GetTypes()
                .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract),
        ];
    }
}
