namespace Caravel.Abstractions;

public interface IEdge
{
    INode Neighbor { get; init; }
    INode Origin { get; init; }
    Func<CancellationToken, Task<INode>> GetNext { get; init; }
    int Weight { get; init; }

    void Deconstruct(
        out INode origin,
        out INode neighbor,
        out Func<CancellationToken, Task<INode>> getNext,
        out int weight
    );
}
