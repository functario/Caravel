namespace Caravel.Abstractions;

public interface IEdge
{
    Type Neighbor { get; }
    Type Origin { get; }
    Func<IJourney, CancellationToken, Task<INode>> MoveNext { get; }
    int Weight { get; }
}
