namespace Caravel.Abstractions;

public interface INeighborNavigator
{
    public IEdgeMetaData? MetaData { get; }
    public Func<IJourney, CancellationToken, Task<INode>> MoveNext { get; }
}
