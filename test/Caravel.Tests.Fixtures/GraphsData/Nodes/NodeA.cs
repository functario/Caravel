using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeA : INode
{
    public NodeA() { }

    public ImmutableHashSet<IEdge> GetEdges(Lazy<IJourney> journey)
    {
        return [this.CreateEdge<NodeB>(journey, OpenNodeB)];
    }

    public Task<NodeB> OpenNodeB(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeB).Name}");
        return Task.FromResult(new NodeB());
    }
}
