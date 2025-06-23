using System.Collections.Frozen;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeA : INode
{
    public NodeA() { }

    public FrozenSet<IEdge> GetEdges()
    {
        return new List<IEdge>() { this.CreateEdge<NodeB>(OpenNodeB) }.ToFrozenSet();
    }

    public Task<NodeB> OpenNodeB(CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeB).Name}");
        return Task.FromResult(new NodeB());
    }
}
