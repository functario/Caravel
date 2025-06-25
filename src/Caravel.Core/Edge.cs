using Caravel.Abstractions;

namespace Caravel.Core;

public record Edge(
    Type Origin,
    Type Neighbor,
    Func<IJourney, CancellationToken, Task<INode>> MoveNext,
    int Weight = 0
) : IEdge
{
    public override string ToString() => $"{Origin.Name} -->|{Weight}| {Neighbor.Name}";
}
