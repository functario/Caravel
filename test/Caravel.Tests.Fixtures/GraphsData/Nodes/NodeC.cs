using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeC : IAuditableNode
{
    private readonly bool _existValue;

    public NodeC(bool existValue = true)
    {
        _existValue = existValue;
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [this.CreateEdge<NodeA>(OpenNodeA)];
    }

    public Task<bool> Audit(IJourney journey, CancellationToken cancellationToken) => Task.FromResult(_existValue);

    public Task<NodeA> OpenNodeA(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeA).Name}");
        return Task.FromResult(new NodeA());
    }
}
