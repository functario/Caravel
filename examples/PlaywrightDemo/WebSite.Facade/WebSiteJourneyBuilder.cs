using Caravel.Abstractions;
using Caravel.Core;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourneyBuilder
{
    private readonly IStartingPOM _current;
    private readonly IGraph _graph;
    private readonly ICoreFactories _coreFactories;
    private readonly InMemoryJourneyLegPublisher _journeyLegPublisher;
    private readonly App _app;
    private readonly Map _map;

    public WebSiteJourneyBuilder(
        IStartingPOM current,
        IGraph graph,
        ICoreFactories coreFactories,
        InMemoryJourneyLegPublisher journeyLegPublisher,
        App app,
        Map map
    )
    {
        _current = current;
        _graph = graph;
        _coreFactories = coreFactories;
        _journeyLegPublisher = journeyLegPublisher;
        _app = app;
        _map = map;
    }

    public WebSiteJourney Create(CancellationToken cancellationToken = default)
    {
        return new WebSiteJourney(
            _current,
            _graph,
            _coreFactories,
            _journeyLegPublisher,
            _app,
            _map,
            cancellationToken
        );
    }
}
