using System.Collections.Immutable;
using System.Diagnostics;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeA : INode
{
    private readonly bool _isOnOpenedSuccess;

    public NodeA(bool isOnOpenedSuccess = true)
    {
        _isOnOpenedSuccess = isOnOpenedSuccess;
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

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        if (_isOnOpenedSuccess)
        {
            return Task.CompletedTask;
        }

        throw new InvalidOperationException(nameof(NodeA));
    }

    public Task<NodeB> OpenNodeB(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeB).Name}");
        return Task.FromResult(new NodeB());
    }

    public Task<NodeD> OpenNodeD(IJourney journey, CancellationToken _)
    {
        Debug.WriteLine($"{GetType().Name} to {typeof(NodeD).Name}");
        return Task.FromResult(new NodeD());
    }
}
