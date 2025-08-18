using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Core;

public sealed record NodeExtended<TNode>(TNode Node, IActionMetaData ActionMetaData) : INode
    where TNode : INode
{
    public ImmutableHashSet<IEdge> GetEdges() => Node.GetEdges();

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken) =>
        Node.OnNodeOpenedAsync(journey, cancellationToken);
}
