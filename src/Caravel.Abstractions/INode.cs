using System.Collections.Frozen;

namespace Caravel.Abstractions;
public interface INode
{
    public FrozenSet<IEdge> GetEdges();
}
