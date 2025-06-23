using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeD : INode
{
    public NodeD() { }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [this.CreateEdge<NodeC>(OpenNodeC)];
    }

    public Task<NodeC> OpenNodeC(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeA).Name}");
        return Task.FromResult(new NodeC());
    }
}
