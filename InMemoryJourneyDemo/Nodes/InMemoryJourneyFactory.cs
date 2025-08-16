using Caravel.Core;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;
using InMemoryJourneyDemo.Nodes.WeightedNodes;

namespace InMemoryJourneyDemo.Nodes;

internal static class InMemoryJourneyFactory
{
    public static InMemoryJourney CreateInMemoryJourney(this UnweightedJourneySeed seed)
    {
        return new InMemoryJourney(
            seed.Node1,
            seed.Graph,
            TimeProvider.System,
            CancellationToken.None
        );
    }

    public static InMemoryJourney CreateInMemoryJourney(this WeightedJourneySeed seed)
    {
        return new InMemoryJourney(
            seed.WeightedNode1,
            seed.WeightedGraph,
            TimeProvider.System,
            CancellationToken.None
        );
    }
}
