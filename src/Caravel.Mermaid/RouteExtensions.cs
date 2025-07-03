using System.Globalization;
using System.Text;
using Caravel.Abstractions;

namespace Caravel.Mermaid;

public static class RouteExtensions
{
    public static string ToMermaidMarkdown(this IRoute route, MermaidOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(route, nameof(route));
        var stringBuilder = new StringBuilder();
        options ??= MermaidOptions.Default();
        stringBuilder.AppendLine(
            CultureInfo.InvariantCulture,
            $"graph {options.GraphDirection.ToString()}"
        );

        IEdge[] edges = [.. route.Edges.OrderBy(x => x.Neighbor.Name)];

        for (var i = 0; i < edges.Length; i++)
        {
            var edgeStr = edges[i].ToString();
            stringBuilder.AppendLine(edgeStr);
        }

        var result = stringBuilder.ToString();
        return result.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase)
            ? result[..^Environment.NewLine.Length]
            : result;
    }
}
