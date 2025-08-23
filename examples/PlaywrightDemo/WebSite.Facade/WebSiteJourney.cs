using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;
using Caravel.Core;
using WebSite.Facade.POMs.Abstractions;

namespace WebSite.Facade;

public sealed class WebSiteJourney : Journey
{
    public WebSiteJourney(
        IStartingPOM current,
        IGraph graph,
        IJourneyConfiguration journeyConfiguration,
        App app,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, journeyConfiguration, journeyCancellationToken)
    {
        App = app;
        Map = map;
    }

    public App App { get; }
    public Map Map { get; }
}
