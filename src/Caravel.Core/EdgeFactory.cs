using Caravel.Abstractions;

namespace Caravel.Core;

public sealed class EdgeFactory : IEdgeFactory
{
    public IEdge GetSelfEdge(INode selfNode)
    {
        ArgumentNullException.ThrowIfNull(selfNode, nameof(selfNode));
        var type = selfNode.GetType();
        return new Edge(type, type, new NeighborNavigator((_, _) => Task.FromResult(selfNode)));
    }
}
