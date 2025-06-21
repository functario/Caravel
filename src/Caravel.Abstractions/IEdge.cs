namespace Caravel.Abstractions;
public interface IEdge
{
    INode Neighbor { get; init; }
    INode Origin { get; init; }
    int Weight { get; init; }

    void Deconstruct(out INode origin, out INode neighbor, out int weight);
}
