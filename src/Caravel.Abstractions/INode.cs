using System.Collections.Immutable;

namespace Caravel.Abstractions;
public interface INode
{
    public ImmutableHashSet<IEdge> GetEdges();
}
