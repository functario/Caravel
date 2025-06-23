using Caravel.Abstractions;

namespace Caravel.Core.Extensions;
public static class NodeExtensions
{
    public static Edge CreateEdge<TNeighbor>(this INode origin, Func<CancellationToken, Task> getNext)
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        var neighborType = typeof(TNeighbor);
        var originType = origin.GetType();
        return new Edge(originType, neighborType, getNext);
    }

}
