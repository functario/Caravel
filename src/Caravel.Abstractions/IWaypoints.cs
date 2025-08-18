namespace Caravel.Abstractions;

public interface IWaypoints : IEnumerable<Type>
{
    int Count { get; }

    // Indexer property
    Type this[int index] { get; }
}
