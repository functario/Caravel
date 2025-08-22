using Caravel.Abstractions;
using Caravel.Core;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;
using InMemoryJourneyDemo.Nodes.WeightedNodes;

namespace InMemoryJourneyDemo.Nodes;

internal static class InMemoryJourneyFactory
{
    public static IJourney CreateInMemoryJourney(
        this UnweightedJourneySeed seed,
        InMemoryJourneyLegPublisher inMemoryJourneyLegPublisher
    )
    {
        return new Journey(
            seed.Node1,
            seed.Graph,
            seed.CoreFactories,
            inMemoryJourneyLegPublisher,
            inMemoryJourneyLegPublisher,
            CancellationToken.None
        );
    }

    public static IJourney CreateInMemoryJourney(
        this WeightedJourneySeed seed,
        InMemoryJourneyLegPublisher inMemoryJourneyLegPublisher
    )
    {
        return new Journey(
            seed.WeightedNode1,
            seed.WeightedGraph,
            seed.CoreFactories,
            inMemoryJourneyLegPublisher,
            inMemoryJourneyLegPublisher,
            CancellationToken.None
        );
    }
}
