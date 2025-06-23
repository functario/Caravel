using System.Collections.Frozen;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeB : INode
{
    public NodeB()
    {
    }

    public FrozenSet<IEdge> GetEdges()
    {
        return new List<IEdge>()
        {
            this.CreateEdge<NodeC>(OpenNodeC)
        }.ToFrozenSet();
    }

    public Task OpenNodeC(CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeC).Name}");
        return Task.CompletedTask;
    }
}
