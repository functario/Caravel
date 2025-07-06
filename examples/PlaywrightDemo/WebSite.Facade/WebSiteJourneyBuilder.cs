using Caravel.Abstractions;
using Microsoft.Extensions.Hosting;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourneyBuilder : IDisposable
{
    private readonly IStartingPOM _current;
    private readonly IGraph _graph;
    private readonly TimeProvider _timeProvider;
    private readonly App _app;
    private readonly Map _map;
    private readonly IHost _host;

    public WebSiteJourneyBuilder(
        IStartingPOM current,
        IGraph graph,
        TimeProvider timeProvider,
        App app,
        Map map,
        IHost host
    )
    {
        _current = current;
        _graph = graph;
        _timeProvider = timeProvider;
        _app = app;
        _map = map;
        _host = host;
    }

    public WebSiteJourney Create(CancellationToken cancellationToken = default)
    {
        return new WebSiteJourney(_current, _graph, _timeProvider, _app, _map, cancellationToken);
    }

    public void Dispose()
    {
        _host?.Dispose();
    }
}
