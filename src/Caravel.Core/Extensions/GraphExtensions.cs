using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class GraphExtensions
{
    public static IRoute GetShortestRoute(this IGraph graph, Type origin, Type destination)
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetShortestRoute(
            origin,
            destination,
            Waypoints.Empty(),
            ExcludedNodes.Empty()
        );
    }

    public static IRoute GetShortestRoute(
        this IGraph graph,
        Type origin,
        Type destination,
        IExcludedNodes excludedNodes
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetShortestRoute(origin, destination, Waypoints.Empty(), excludedNodes);
    }

    public static IRoute GetShortestRoute(
        this IGraph graph,
        Type origin,
        Type destination,
        IWaypoints waypoints
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        return graph.GetShortestRoute(origin, destination, waypoints, ExcludedNodes.Empty());
    }

    public static string ToMermaidGraph(
        this IGraph graph,
        MermaidGraphDirections mermaidGraphDirection = default
    )
    {
        ArgumentNullException.ThrowIfNull(graph, nameof(graph));
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(
            CultureInfo.InvariantCulture,
            $"graph {mermaidGraphDirection.ToString()}"
        );

        // order need to be respected for comparison
        List<(Type node, ImmutableHashSet<IEdge> edges)> nodeEdges =
        [
            .. graph.Nodes.OrderBy(x => x.Key.Name).Select(n => (node: n.Key, edges: n.Value.GetEdges())),
        ];

        for (var i = 0; i < nodeEdges.Count; i++)
        {
            foreach (var edge in nodeEdges[i].edges.OrderBy(x => x.Neighbor.Name))
            {
                var edgeStr = edge.ToString();
                stringBuilder.AppendLine(edgeStr);
            }
        }

        var result = stringBuilder.ToString();
        return result.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase)
            ? result[..^Environment.NewLine.Length]
            : result;
        ;
    }
}
