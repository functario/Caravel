using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class NodeExtensions
{
    public static Edge CreateEdge<TNeighbor>(
        this INode origin,
        Func<CancellationToken, Task<TNeighbor>> getNext
    )
        where TNeighbor : INode
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        var neighborType = typeof(TNeighbor);
        var originType = origin.GetType();

        async Task<INode> Wrapped(CancellationToken ct) =>
            await getNext(ct).ConfigureAwait(false);

        return new Edge(originType, neighborType, Wrapped);
    }
}
