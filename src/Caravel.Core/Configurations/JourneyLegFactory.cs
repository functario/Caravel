using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;

namespace Caravel.Core.Configurations;

public sealed class JourneyLegFactory : IJourneyLegFactory
{
    public IJourneyLeg CreateJourneyLeg(
        INode currentNode,
        INode destination,
        Guid journeyId,
        IRouteFactory routeFactory,
        IEdgeFactory edgeFactory,
        IActionMetaData? actionMetaData
    )
    {
        ArgumentNullException.ThrowIfNull(edgeFactory, nameof(edgeFactory));
        ArgumentNullException.ThrowIfNull(routeFactory, nameof(routeFactory));

        var edge = edgeFactory.CreateEdge(currentNode, destination, actionMetaData);
        var legEdges = new Queue<IEdge>([edge]);
        var doRoute = routeFactory.CreateRoute([.. legEdges]);
        return CreateJourneyLeg(journeyId, legEdges, doRoute);
    }

    public IJourneyLeg CreateJourneyLeg(Guid journeyId, Queue<IEdge> legEdges, IRoute route)
    {
        return new JourneyLeg(journeyId, legEdges, route);
    }
}
