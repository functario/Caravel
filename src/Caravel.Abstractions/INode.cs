using System.Collections.Frozen;

namespace Caravel.Abstractions;
public interface INode
{
    string Name { get; }
    FrozenSet<IEdge> Edges { get; }
}
