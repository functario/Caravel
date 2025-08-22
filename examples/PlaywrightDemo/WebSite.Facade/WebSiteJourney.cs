using Caravel.Abstractions;
using Caravel.Core;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourney : InMemoryJourney
{
    public WebSiteJourney(
        IStartingPOM current,
        IGraph graph,
        TimeProvider timeProvider,
        App app,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, timeProvider, journeyCancellationToken)
    {
        App = app;
        Map = map;
    }

    public App App { get; }
    public Map Map { get; }
}
