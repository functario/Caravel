using Caravel.Abstractions;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourneyBuilder
{
    private readonly IStartingPOM _current;
    private readonly IGraph _graph;
    private readonly IJourneyCoreOptions _journeyCoreOptions;
    private readonly App _app;
    private readonly Map _map;

    public WebSiteJourneyBuilder(
        IStartingPOM current,
        IGraph graph,
        IJourneyCoreOptions journeyCoreOptions,
        App app,
        Map map
    )
    {
        _current = current;
        _graph = graph;
        _journeyCoreOptions = journeyCoreOptions;
        _app = app;
        _map = map;
    }

    public WebSiteJourney Create(CancellationToken cancellationToken = default)
    {
        return new WebSiteJourney(
            _current,
            _graph,
            _journeyCoreOptions,
            _app,
            _map,
            cancellationToken
        );
    }
}
