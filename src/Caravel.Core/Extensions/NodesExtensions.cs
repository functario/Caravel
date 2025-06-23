using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class NodesExtensions
{
    public static IEdge GetEdge<TOrigin, TNeighbor>(this IEnumerable<INode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));
        var nodeMatches = nodes.Where(n => n is TOrigin).ToArray();

        var edges = nodeMatches.Length switch
        {
            1 => nodeMatches[0].GetEdges(new Lazy<IJourney>()),
            < 1 => throw new InvalidOperationException(
                $"There is no orign {nameof(INode)} of type '{typeof(TOrigin)}'."
            ),
            > 1 => throw new InvalidOperationException(
                $"There are {nodeMatches.Length} nodes of type '{typeof(TOrigin)}'."
            ),
        };

        if (nodeMatches is null)
        {
            throw new InvalidOperationException($"There is no orign node of type '{typeof(TOrigin)}'.");
        }

        var edgeMatches = edges.Where(e => e.Neighbor == typeof(TNeighbor)).ToArray();

        var edge = edgeMatches.Length switch
        {
            1 => edgeMatches[0],
            < 1 => throw new InvalidOperationException(
                $"There is no {nameof(IEdge)} from origin '{typeof(TOrigin)}' with neighbor '{typeof(TNeighbor)}'."
            ),
            > 1 => throw new InvalidOperationException(
                $"There are {nodeMatches.Length} {nameof(IEdge)} from origin '{typeof(TOrigin)}' with neighbor '{typeof(TNeighbor)}'."
            ),
        };

        return edge;
    }
}
