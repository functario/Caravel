using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeB : INode
{
    private readonly bool _isOnOpenedSuccess;

    public NodeB(bool isOnOpenedSuccess = true)
    {
        _isOnOpenedSuccess = isOnOpenedSuccess;
    }

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        if (_isOnOpenedSuccess)
        {
            return Task.CompletedTask;
        }

        throw new InvalidOperationException(nameof(NodeB));
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return
        [
            this.CreateEdge<NodeC>(OpenNodeC)
        ];
    }

    public Task<NodeC> OpenNodeC(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeC).Name}");
        return Task.FromResult(new NodeC());
    }
}
