using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Graph.Dijkstra;

public class Route : IRoute
{
    public ImmutableList<IEdge> Edges { get; }

    public Route(ImmutableList<IEdge> edges)
    {
        Edges = edges;
    }
}
