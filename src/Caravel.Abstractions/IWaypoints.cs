namespace Caravel.Abstractions;

public interface IWaypoints : IEnumerable<Type>
{
    int Count { get; }
    Type this[int index] { get; }
}
