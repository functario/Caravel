using Caravel.Abstractions;

namespace Caravel.Core;

public record NeighborNavigator(
    Func<IJourney, CancellationToken, Task<INode>> MoveNext,
    IActionMetaData? MetaData = null
) : INeighborNavigator
{ }
