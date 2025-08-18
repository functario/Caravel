using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Core;

/// <inheritdoc cref="IEnrichedNode{TNode}" />
public sealed record EnrichedNode<TNode>(TNode NodeToEnrich, IActionMetaData ActionMetaData)
    : IEnrichedNode<TNode>
    where TNode : INode
{
    /// <inheritdoc cref="INode" />
    public ImmutableHashSet<IEdge> GetEdges() => NodeToEnrich.GetEdges();

    /// <inheritdoc cref="INode" />
    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken) =>
        NodeToEnrich.OnNodeOpenedAsync(journey, cancellationToken);
}
