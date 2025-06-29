using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeC : INode
{
    private readonly bool _isOnOpenedSuccess;

    public NodeC(bool isOnOpenedSuccess = true)
    {
        _isOnOpenedSuccess = isOnOpenedSuccess;
    }

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        if (_isOnOpenedSuccess)
        {
            return Task.CompletedTask;
        }

        throw new InvalidOperationException(nameof(NodeC));
    }

    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [this.CreateEdge<NodeA>(OpenNodeA), this.CreateEdge<NodeE>(OpenNodeE)];
    }


    public Task<NodeA> OpenNodeA(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeA).Name}");
        return Task.FromResult(new NodeA());
    }

    public Task<NodeE> OpenNodeE(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeE).Name}");
        return Task.FromResult(new NodeE());
    }
}
