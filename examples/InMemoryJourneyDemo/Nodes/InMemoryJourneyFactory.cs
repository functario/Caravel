using Caravel.Abstractions;
using Caravel.Core;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;
using InMemoryJourneyDemo.Nodes.WeightedNodes;

namespace InMemoryJourneyDemo.Nodes;

internal static class InMemoryJourneyFactory
{
    public static IJourney CreateJourney(this UnweightedJourneySeed seed)
    {
        return new Journey(
            seed.Node1,
            seed.Graph,
            seed.JourneyConfiguration,
            CancellationToken.None
        );
    }

    public static IJourney CreateJourney(this WeightedJourneySeed seed)
    {
        return new Journey(
            seed.WeightedNode1,
            seed.WeightedGraph,
            seed.JourneyConfiguration,
            CancellationToken.None
        );
    }
}
