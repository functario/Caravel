using System.Collections.Frozen;

namespace Caravel.Abstractions;

public interface IGraph
{
    IRoute GetShortestRoute(
        Type origin,
        Type destination,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes
    );

    FrozenDictionary<Type, INode> Nodes { get; }
}
