using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeA : IAuditableNode
{
    private readonly bool _existValue;

    public NodeA(bool existValue = true)
    {
        _existValue = existValue;
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        // csharpier-ignore
        return
            [
            this.CreateEdge<NodeB>(OpenNodeB),
            this.CreateEdge<NodeD>(OpenNodeD, 1)
            ];
    }

    public Task<bool> Audit(IJourney journey, CancellationToken cancellationToken) => Task.FromResult(_existValue);

    public Task<NodeB> OpenNodeB(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeB).Name}");
        return Task.FromResult(new NodeB());
    }

    public Task<NodeD> OpenNodeD(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeB).Name}");
        return Task.FromResult(new NodeD());
    }
}
