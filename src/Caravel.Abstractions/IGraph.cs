namespace Caravel.Abstractions;
public interface IGraph
{
    IRoute GetShortestRoute(Type origin, Type destination, params Type[] waypoints);
    IRoute GetShortestRoute(Type origin, Type destination);
}
