using System.Collections.Frozen;

namespace Caravel.Abstractions;
public interface INode
{
    string Name { get; }
    public FrozenSet<IEdge> GetEdges();
}
