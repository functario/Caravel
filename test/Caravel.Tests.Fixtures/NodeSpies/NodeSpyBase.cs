using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures.NodeSpies;

public class NodeSpyBase : INodeSpy
{
    private readonly bool _existValue;

    public NodeSpyBase(ImmutableHashSet<IEdge> edges, bool existValue = true)
    {
        _existValue = existValue;
        InternalEdges = edges;
    }


    public ImmutableHashSet<IEdge> InternalEdges { get; init; } = [];

    public ImmutableHashSet<IEdge> GetEdges() => InternalEdges;


    public Task<bool> AuditAsync(IJourney journey, CancellationToken cancellationToken) => Task.FromResult(_existValue);
}
