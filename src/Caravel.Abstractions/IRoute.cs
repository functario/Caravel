using System.Collections.Frozen;

namespace Caravel.Abstractions;
public interface IRoute
{
    FrozenSet<IEdge> Edges { get; }
}
