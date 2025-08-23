using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;

namespace Caravel.Core.Configurations;

public sealed class EdgeFactory : IEdgeFactory
{
    public IEdge CreteSelfEdge(INode selfNode)
    {
        ArgumentNullException.ThrowIfNull(selfNode, nameof(selfNode));
        var type = selfNode.GetType();
        var neighborNavigator = new NeighborNavigator((_, _) => Task.FromResult(selfNode));
        return new Edge(type, type, neighborNavigator);
    }

    public IEdge CreateEdge(INode origin, INode destination, IActionMetaData? actionMetaData)
    {
        ArgumentNullException.ThrowIfNull(origin, nameof(origin));
        ArgumentNullException.ThrowIfNull(destination, nameof(destination));
        var neighborNavigator = new NeighborNavigator(
            (_, _) => Task.FromResult(destination),
            actionMetaData
        );

        return new Edge(origin.GetType(), destination.GetType(), neighborNavigator);
    }
}
