using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Core.Extensions;
using Caravel.Mermaid;

namespace InMemoryJourneyDemo.Nodes.WeightedNodes;

// csharpier-ignore-start
[GridPosition(row: 2, column: 1)]
internal sealed class WeightedNode3 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        return [
            this.CreateEdge(OpenNode1,
                weight: 10,
                metaData: new ActionMetaData("Open the Node1 from Node3"))
            ];
    }

    private static Task<WeightedNode1> OpenNode1(IJourney journey, CancellationToken cancellationToken) =>
        Task.FromResult(new WeightedNode1());

    public Task OnNodeVisitedAsync(IJourney journey, CancellationToken cancellationToken)
    {
        // Add your own logic
        return Task.CompletedTask;
    }
}
// csharpier-ignore-end
