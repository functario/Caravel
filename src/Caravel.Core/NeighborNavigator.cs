using Caravel.Abstractions;

namespace Caravel.Core;

public record NeighborNavigator(
    Func<IJourney, CancellationToken, Task<INode>> MoveNext,
    IEdgeMetaData? MetaData = null
) : INeighborNavigator
{ }
