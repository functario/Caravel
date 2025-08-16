using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace InMemoryJourneyDemo.Nodes.UnweightedNodes;

// csharpier-ignore-start
internal sealed class Node3 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [
            this.CreateEdge(OpenNode1)
            ];
    }

    private static Task<Node1> OpenNode1(IJourney journey, CancellationToken cancellationToken) =>
        Task.FromResult(new Node1());

    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        // Add your own logic
        return Task.CompletedTask;
    }
}
// csharpier-ignore-end
