namespace Caravel.Abstractions;

/// <summary>
/// Wrapper around <see cref="NodeToEnrich"/> to add information during navigation of a scoped <see cref="IJourney"/>.
/// </summary>
/// <remarks>It should not be used as a declaring type for <see cref="INode"/>
/// and should only be used from the context of a scoped <see cref="IJourney"/>.</remarks>
/// <typeparam name="TNode">The enriched <see cref="INode"/> type.</typeparam>
public interface IEnrichedNode<TNode> : INode
    where TNode : INode
{
    /// <summary>
    /// The <see cref="ActionMetaData"/> to add when returns from <see cref="IJourney.DoAsync{TCurrentNode, TNodeOut}(Func{IJourney, TCurrentNode, CancellationToken, Task{TNodeOut}}, CancellationToken)"/>
    /// </summary>
    ///
    IActionMetaData ActionMetaData { get; init; }

    /// <summary>
    /// The <see cref="INode"/> to enrich.
    /// </summary>
    TNode NodeToEnrich { get; init; }
}
