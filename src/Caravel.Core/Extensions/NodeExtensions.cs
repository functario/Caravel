﻿using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class NodeExtensions
{
    /// <summary>
    /// Create an <see cref="IEdge"/>.
    /// </summary>
    /// <typeparam name="TNeighbor">The type of the neighbor. It is mandatory and cannot be inferred.</typeparam>
    /// <param name="origin">The origin <see cref="INode"/>.</param>
    /// <param name="moveNext">The function to move to the neighbor.</param>
    /// <param name="weight">The weight of the <see cref="IEdge"/>.</param>
    /// <param name="metaData">The metaData of the <see cref="IEdge.NeighborNavigator"/>.</param>
    /// <param name="_">Unused parameter preventing inference to ensure match between <typeparamref name="TNeighbor"/> and <paramref name="moveNext"/>.</param>
    /// <returns>The <see cref="IEdge"/>.</returns>
    public static IEdge CreateEdge<TNeighbor>(
        this INode origin,
        Func<IJourney, CancellationToken, Task<TNeighbor>> moveNext,
        int weight = 0,
        object? metaData = null,
        TNeighbor _ = default!
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
