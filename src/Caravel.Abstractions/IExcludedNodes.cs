namespace Caravel.Abstractions;

public interface IExcludedNodes : IEnumerable<Type>
{
    int Count { get; }
    Type this[int index] { get; }
}
