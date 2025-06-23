using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeC : INode
{
    public NodeC() { }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [this.CreateEdge<NodeA>(OpenNodeA)];
    }

    public Task<NodeA> OpenNodeA(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeA).Name}");
        return Task.FromResult(new NodeA());
    }
}
