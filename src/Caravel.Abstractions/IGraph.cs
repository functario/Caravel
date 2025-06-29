using System.Collections.Frozen;

namespace Caravel.Abstractions;

public interface IGraph
{
    IRoute GetRoute(
        Type origin,
        Type destination,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes
    );

    FrozenDictionary<Type, INode> Nodes { get; }
}
