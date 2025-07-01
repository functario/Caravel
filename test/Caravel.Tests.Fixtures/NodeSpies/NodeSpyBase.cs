using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures.NodeSpies;

public class NodeSpyBase : INodeSpy
{
    private readonly bool _isOnOpenedSuccess;

    public NodeSpyBase(ImmutableHashSet<IEdge> edges, bool isOnOpenedSuccess = true)
    {
        _isOnOpenedSuccess = isOnOpenedSuccess;
        InternalEdges = edges;
    }

    public ImmutableHashSet<IEdge> InternalEdges { get; init; } = [];

    public ImmutableHashSet<IEdge> GetEdges() => InternalEdges;

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_isOnOpenedSuccess)
        {
            return Task.CompletedTask;
        }

        throw new InvalidOperationException();
    }
}
