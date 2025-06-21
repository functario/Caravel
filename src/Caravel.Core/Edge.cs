using Caravel.Abstractions;

namespace Caravel.Core;

public record Edge(INode Origin, INode Neighbor, int Weight = 0)
{
    public override string ToString() => $"{Origin.Name} =>{Weight} {Neighbor.Name}";
}
