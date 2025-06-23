namespace Caravel.Abstractions;

public interface IEdge
{
    Type Neighbor { get; }
    Type Origin { get; }
    Func<CancellationToken, Task<INode>> MoveNext { get; }
    int Weight { get; }
}
