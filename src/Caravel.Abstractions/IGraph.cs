namespace Caravel.Abstractions;
public interface IGraph
{
    IRoute GetShortestRoute(Lazy<IJourney> journey, Type origin, ICollection<Type> waypoints, Type destination);
    IRoute GetShortestRoute(Lazy<IJourney> journey, Type origin, Type destination);
}
