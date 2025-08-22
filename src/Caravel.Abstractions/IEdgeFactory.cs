namespace Caravel.Abstractions;

public interface IEdgeFactory
{
    /// <summary>
    /// Create a <see cref="IEdge"/> for a <see cref="INode"/> navigating to itself.
    /// The <see cref="IRoute.Edges"/> contains only the <see cref="INode"/> self reference.
    /// </summary>
    /// <param name="selfNode">The Node navigating to itself.</param>
    /// <returns>The <see cref="IEdge"/>.</returns>
    /// <remarks>The <see cref="INeighborNavigator"/>The <see cref="IEdge.NeighborNavigator"/>
    /// returns from the route returns an instance pass in parameters.</remarks>
    public IEdge GetSelfEdge(INode selfNode);
}
