namespace Caravel.Abstractions;
public interface IGraph
{
    IRoute GetShortestRoute(Type origin, ICollection<Type> waypoints, Type destination);
    IRoute GetShortestRoute(Type origin, Type destination);
}
