namespace Caravel.Abstractions;

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

    public IEdge CreateEdge(INode origin, INode destination, IActionMetaData? actionMetaData);
}
