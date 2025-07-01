using Caravel.Abstractions;

namespace Caravel.Core;

public record Edge(Type Origin, Type Neighbor, INeighborNavigator NeighborNavigator, int Weight = 0)
    : IEdge { }
