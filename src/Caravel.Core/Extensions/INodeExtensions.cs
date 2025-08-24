using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class INodeExtensions
{
    /// <summary>
    /// Create an <see cref="Edge"/>.
    /// </summary>
    /// <typeparam name="TNeighbor">The type of the neighbor</typeparam>
    /// <param name="origin">The origin <see cref="INode"/>.</param>
    /// <param name="moveNext">The function to move to the neighbor.</param>
    /// <param name="weight">The weight of the <see cref="IEdge"/>.</param>
    /// <param name="metaData">The metaData to add to the <see cref="NeighborNavigator"/>.</param>
    /// <returns>The <see cref="IEdge"/>.</returns>
    /// <remarks>This helper method used the default <see cref="Edge"/> and <see cref="NeighborNavigator"/>.
    /// Do not use for custom <see cref="IEdge"/> or <see cref="INeighborNavigator"/>.
    /// </remarks>
    public static IEdge CreateEdge<TNeighbor>(
        this INode origin,
        Func<IJourney, CancellationToken, Task<TNeighbor>> moveNext,
        int weight = 0,
        IActionMetaData? metaData = null
    )
        where TNeighbor : INode
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        var neighborType = typeof(TNeighbor);
        var originType = origin.GetType();

        async Task<INode> WrappedMoveNExt(IJourney journey, CancellationToken ct) =>
            await moveNext(journey, ct).ConfigureAwait(false);

        var neighborNavigator = new NeighborNavigator(WrappedMoveNExt, metaData);
        return new Edge(originType, neighborType, neighborNavigator, weight);
    }
}
