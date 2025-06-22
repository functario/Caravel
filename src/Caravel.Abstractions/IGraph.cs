namespace Caravel.Abstractions;
public interface IGraph
{
    IRoute GetShortestRoute(INode origin, ICollection<INode> waypoints, INode destination);
    IRoute GetShortestRoute(INode origin, INode destination);
}
