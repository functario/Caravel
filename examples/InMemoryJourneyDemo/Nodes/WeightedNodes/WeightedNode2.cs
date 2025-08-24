using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Core.Extensions;
using Caravel.Mermaid;

namespace InMemoryJourneyDemo.Nodes.WeightedNodes;

// csharpier-ignore-start
[GridPosition(row: 1, column: 0)]
internal sealed class WeightedNode2 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [
            this.CreateEdge(
                OpenNode3,
                weight: 1,
                metaData: new ActionMetaData("Open the Node3 from Node2"))
            ];
    }

    private static Task<WeightedNode3> OpenNode3(IJourney journey, CancellationToken cancellationToken) =>
        Task.FromResult(new WeightedNode3());

    public Task OnNodeVisitedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        // Add your own logic
        return Task.CompletedTask;
    }
}
// csharpier-ignore-end
