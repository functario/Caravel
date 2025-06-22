using System.Collections.Frozen;
using Caravel.Abstractions;

namespace Caravel.Core;

public class Route : IRoute
{
    public FrozenSet<IEdge> Edges { get; }

    public Route(FrozenSet<IEdge> edges)
    {
        Edges = edges;
    }
}
