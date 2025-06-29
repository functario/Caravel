namespace Caravel.Abstractions;

public interface IEdge
{
    Type Neighbor { get; }
    Type Origin { get; }
    INeighborNavigator NeighborNavigator { get; }
    int Weight { get; }
}
