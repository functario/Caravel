namespace Caravel.Abstractions;

public interface IJourneyLegFactory
{
    public IJourneyLeg CreateJourneyLeg(
        INode currentNode,
        INode destination,
        Guid journeyId,
        IRouteFactory routeFactory,
        IEdgeFactory edgeFactory,
        IActionMetaData? actionMetaData
    );

    public IJourneyLeg CreateJourneyLeg(Guid journeyId, Queue<IEdge> legEdges, IRoute route);
}
