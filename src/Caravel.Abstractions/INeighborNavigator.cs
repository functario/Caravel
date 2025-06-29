namespace Caravel.Abstractions;

public interface INeighborNavigator
{
    public object? MetaData { get; }
    public Func<IJourney, CancellationToken, Task<INode>> MoveNext { get; }
}
