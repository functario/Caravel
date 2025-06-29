using System.Collections.Immutable;

namespace Caravel.Abstractions;
public interface INode
{
    public ImmutableHashSet<IEdge> GetEdges();
    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken);
}
