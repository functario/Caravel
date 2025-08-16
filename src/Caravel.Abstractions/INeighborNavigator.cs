namespace Caravel.Abstractions;

public interface INeighborNavigator
{
    public IActionMetaData? MetaData { get; }
    public Func<IJourney, CancellationToken, Task<INode>> MoveNext { get; }
}
