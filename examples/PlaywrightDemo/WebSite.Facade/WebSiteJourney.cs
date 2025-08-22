using Caravel.Abstractions;
using Caravel.Core;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourney : Journey
{
    public WebSiteJourney(
        IStartingPOM current,
        IGraph graph,
        IJourneyCoreOptions journeyCoreOptions,
        App app,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, journeyCoreOptions, journeyCancellationToken)
    {
        App = app;
        Map = map;
    }

    public App App { get; }
    public Map Map { get; }
}
