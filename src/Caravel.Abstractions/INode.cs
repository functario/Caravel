using System.Collections.Immutable;

namespace Caravel.Abstractions;
public interface INode
{
    public ImmutableHashSet<IEdge> GetEdges();

    /// <summary>
    /// Method called each time a <see cref="INode"/> is opened.
    /// </summary>
    /// <param name="journey">The current <see cref="IJourney"/>.</param>
    /// <param name="cancellationToken">The CancellationToken.</param>
    /// <returns>A Task.</returns>
    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken);
}
