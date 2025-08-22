using System.Collections.Immutable;

namespace Caravel.Abstractions;

public interface IRouteFactory
{
    /// <summary>
    /// Create a <see cref="IRoute"/>.
    /// </summary>
    /// <param name="edges">The <see cref="IEdge"/> in order of crossing.</param>
    /// <returns>The <see cref="IRoute"/>.</returns>
    public IRoute CreateRoute(IImmutableList<IEdge> edges);
}
