using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public record Journey : IJourney
{
    public Journey(INode current, IGraph graph)
    {
        ArgumentNullException.ThrowIfNull(current, nameof(current));
        Graph = graph;
        Current = current;
        var history = new Queue<Type>([current.GetType()]);
        Log = new DriverLog(history);
    }

    public INode Current { get; private set; }
    public IGraph Graph { get; init; }
    public IJourneyLog Log { get; init; }

    public async Task<IJourney> GotoAsync<TDestination>(CancellationToken methodCancellationToken = default)
        where TDestination : INode
    {
        var originType = Current.GetType();
        var destinationType = typeof(TDestination);
        var shortestRoute = Graph.GetShortestRoute(originType, destinationType);
        var edges = shortestRoute.Edges;
        if (edges.Any(x => x is null))
        {
            throw new InvalidOperationException("Edge should not be null.");
        }

        foreach (var edge in edges)
        {
            Current = await edge.GetNext(methodCancellationToken).ConfigureAwait(false);
            Log.History.Enqueue(Current.GetType());
        }

        if (Current is not TDestination)
        {
            throw new InvalidOperationException("The last INode is not the destination.");
        }

        return this;
    }
}
