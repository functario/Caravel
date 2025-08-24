namespace Caravel.Abstractions.Configurations;

public interface IEdgeFactory
{
    /// <summary>
    /// Create a <see cref="IEdge"/> for a <see cref="INode"/> navigating to itself.
    /// </summary>
    /// <param name="selfNode">The Node navigating to itself.</param>
    /// <returns>The <see cref="IEdge"/>.</returns>
    /// <remarks>The <see cref="INeighborNavigator"/>
    /// returned from the <see cref="IEdge"/> must contains the same instance than <paramref name="selfNode"/>.</remarks>
    public IEdge CreteSelfEdge(INode selfNode);

    /// <summary>
    /// Create a <see cref="IEdge"/> with no weight.
    /// </summary>
    /// <param name="origin">The point of origin.</param>
    /// <param name="destination">The point of destination.</param>
    /// <param name="actionMetaData">The optional <see cref="IActionMetaData"/>.</param>
    /// <returns>An <see cref="IEdge"/> returning an instance of the <paramref name="destination"/>.</returns>
    public IEdge CreateEdge(INode origin, INode destination, IActionMetaData? actionMetaData);
}
