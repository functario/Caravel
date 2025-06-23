using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeB : INode
{
    public NodeB()
    {
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return
        [
            this.CreateEdge<NodeC>(OpenNodeC)
        ];
    }

    public Task<NodeC> OpenNodeC(CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeC).Name}");
        return Task.FromResult(new NodeC());
    }
}
