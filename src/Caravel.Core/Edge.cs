using Caravel.Abstractions;

namespace Caravel.Core;

public record Edge(
    INode Origin,
    INode Neighbor,
    Func<CancellationToken, Task<INode>> GetNext,
    int Weight = 0
) : IEdge
{
    public override string ToString() => $"{Origin.Name} =>{Weight} {Neighbor.Name}";
}
