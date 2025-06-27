using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.DijkstraGraph;

public class Route : IRoute
{
    public ImmutableList<IEdge> Edges { get; }

    public Route(ImmutableList<IEdge> edges)
    {
        Edges = edges;
    }
}
