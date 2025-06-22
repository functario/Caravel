using Caravel.Abstractions;

namespace Caravel.Core;

public static class MapExtensions
{
    public static Edge CreateEdge<TDestination>(
        this IMap map,
        INode origin,
        Func<CancellationToken, Task<TDestination>> getNext
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(map, nameof(map));
        ArgumentNullException.ThrowIfNull(getNext, nameof(getNext));

        var destinations = map.Nodes.Where(n => n.GetType() is TDestination).ToArray();
        var count = destinations.Length;

        if (getNext is Func<CancellationToken, Task<INode>> aaa)
        {
            var destination = destinations.Length switch
            {
                1 => new Edge(origin, destinations.First(), aaa),
                > 1 => throw new InvalidOperationException("Too many destination matches."),
                < 1 => throw new InvalidOperationException("No match found."),
            };

            if (destination is not TDestination)
            {
                throw new InvalidCastException(
                    $"Counld not cast {destination.GetType().Name} to expected type {typeof(TDestination)}."
                );
            }

            return destination;
        }

        throw new InvalidCastException(
            $"Counld not cast {getNext.GetType().Name} to expected type {typeof(Func<CancellationToken, Task<INode>>)}."
        );
    }
}
