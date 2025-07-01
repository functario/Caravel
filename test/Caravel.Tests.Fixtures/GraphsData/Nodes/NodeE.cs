using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeE : INode
{
    private readonly bool _isOnOpenedSuccess;

    public NodeE(bool isOnOpenedSuccess = true)
    {
        _isOnOpenedSuccess = isOnOpenedSuccess;
    }

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        if (_isOnOpenedSuccess)
        {
            return Task.CompletedTask;
        }

        throw new InvalidOperationException(nameof(NodeE));
    }

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
