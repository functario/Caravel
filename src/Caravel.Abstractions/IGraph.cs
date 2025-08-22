using System.Collections.Frozen;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Abstractions;

public interface IGraph
{
    /// <summary>
    /// Resolve a <see cref="IRoute"/> of ordered <see cref="IEdge"/>.
    /// </summary>
    /// <param name="origin">The origin's type.</param>
    /// <param name="destination">The destination's type.</param>
    /// <param name="waypoints">The ordered waypoints to traverse.</param>
    /// <param name="excludedNodes">The <see cref="INode"/> to exclude from the route resolution.</param>
    /// <returns>The <see cref="IRoute"/>.</returns>
    /// <exception cref="RouteNotFoundException"></exception>
    IRoute GetRoute(
        Type origin,
        Type destination,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes
    );

    /// <summary>
    /// Returns a <see cref="IRoute"/> from a Node to itself.
    /// </summary>
    /// <param name="node">The node to returns from the <see cref="IEdge.NeighborNavigator"/>.</param>
    /// <returns>A <see cref="IRoute"/> from the Node to itself without action.</returns>
    IRoute GetSelfRoute(INode node);

    /// <summary>
    /// The declared Nodes to use when resolving the <see cref="IRoute"/>.
    /// </summary>
    FrozenDictionary<Type, INode> Nodes { get; }

    public IRouteFactory RouteFactory { get; }
}
