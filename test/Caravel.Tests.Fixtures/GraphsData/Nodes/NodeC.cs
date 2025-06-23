using System.Collections.Frozen;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeC : INode
{
    public NodeC() { }

    public FrozenSet<IEdge> GetEdges()
    {
        return new List<IEdge>() { this.CreateEdge<NodeA>(OpenNodeA) }.ToFrozenSet();
    }

    public Task OpenNodeA(CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeB).Name}");
        return Task.CompletedTask;
    }
}
