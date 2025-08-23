using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;

namespace Caravel.Graph.Dijkstra;

public sealed class RouteFactory : IRouteFactory
{
    public IRoute CreateRoute(IImmutableList<IEdge> edges) => new Route([.. edges]);
}
