using Caravel.Abstractions;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourneyBuilder
{
    private readonly IStartingPOM _current;
    private readonly IGraph _graph;
    private readonly TimeProvider _timeProvider;
    private readonly App _app;
    private readonly Map _map;
    private readonly IJourneyLegFactory _journeyLegFactory;
    private readonly IActionMetaDataFactory _actionMetaDataFactory;

    public WebSiteJourneyBuilder(
        IStartingPOM current,
        IGraph graph,
        TimeProvider timeProvider,
        App app,
        Map map,
        IJourneyLegFactory journeyLegFactory,
        IActionMetaDataFactory actionMetaDataFactory
    )
    {
        _current = current;
        _graph = graph;
        _timeProvider = timeProvider;
        _app = app;
        _map = map;
        _journeyLegFactory = journeyLegFactory;
        _actionMetaDataFactory = actionMetaDataFactory;
    }

    public WebSiteJourney Create(CancellationToken cancellationToken = default)
    {
        return new WebSiteJourney(
            _current,
            _graph,
            _timeProvider,
            _app,
            _map,
            _journeyLegFactory,
            _actionMetaDataFactory,
            cancellationToken
        );
    }
}
