using System.Globalization;
using System.Text;
using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class RouteExtensions
{
    private const string MermaidGraph = "graph ";

    public static string ToMermaidGraph(
        this IRoute route,
        MermaidGraphDirections mermaidGraphDirection
    )
    {
        ArgumentNullException.ThrowIfNull(route, nameof(route));
        var stingBuilder = new StringBuilder();
        stingBuilder.Append(
            CultureInfo.InvariantCulture,
            $"{MermaidGraph} {mermaidGraphDirection.ToString()}"
        );

        for (var i = 0; i < route.Edges.Count; i++)
        {
            var edgeStr = route.Edges[i].ToString();
            stingBuilder.AppendLine(edgeStr);
        }

        return stingBuilder.ToString();
    }
}
