using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace InMemoryJourneyDemo.Nodes.UnweightedNodes;

// csharpier-ignore-start
internal sealed class Node1 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [
            this.CreateEdge(OpenNode2),
            this.CreateEdge(OpenNode3)
            ];
    }

    private static Task<Node2> OpenNode2(IJourney journey, CancellationToken cancellationToken) =>
        Task.FromResult(new Node2());

    private static Task<Node3> OpenNode3(IJourney journey, CancellationToken cancellationToken) =>
        Task.FromResult(new Node3());

    public Task OnNodeVisitedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        // Add your own logic
        return Task.CompletedTask;
    }
}
// csharpier-ignore-end
