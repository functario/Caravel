using Caravel.Abstractions;
using Caravel.Core;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourney : Journey
{
    public WebSiteJourney(
        IStartingPOM current,
        IGraph graph,
        ICoreFactories coreFactories,
        InMemoryJourneyLegPublisher journeyLegPublisher,
        App app,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(
            current,
            graph,
            coreFactories,
            journeyLegPublisher,
            journeyLegPublisher,
            journeyCancellationToken
        )
    {
        App = app;
        Map = map;
        JourneyLegPublisher = journeyLegPublisher;
    }

    public App App { get; }
    public Map Map { get; }
    public InMemoryJourneyLegPublisher JourneyLegPublisher { get; }
}
