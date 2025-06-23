namespace Caravel.Abstractions;

public interface IEdge
{
    Type Neighbor { get; }
    Type Origin { get; }
    Func<CancellationToken, Task<INode>> GetNext { get; }
    int Weight { get; }
}
