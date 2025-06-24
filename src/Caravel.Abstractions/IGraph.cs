namespace Caravel.Abstractions;
public interface IGraph
{
    IRoute GetShortestRoute(Type origin, Type destination, IWaypoints waypoints, IExcludedNodes excludedNodes);
    IRoute GetShortestRoute(Type origin, Type destination, IWaypoints waypoints);
    IRoute GetShortestRoute(Type origin, Type destination, IExcludedNodes excludedNodes);
    IRoute GetShortestRoute(Type origin, Type destination);
}
