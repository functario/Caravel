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

    public WebSiteJourneyBuilder(
        IStartingPOM current,
        IGraph graph,
        TimeProvider timeProvider,
        App app,
        Map map
    )
    {
        _current = current;
        _graph = graph;
        _timeProvider = timeProvider;
        _app = app;
        _map = map;
    }

    public WebSiteJourney Create(CancellationToken cancellationToken)
    {
        return new WebSiteJourney(_current, _graph, _timeProvider, _app, _map, cancellationToken);
    }
}
