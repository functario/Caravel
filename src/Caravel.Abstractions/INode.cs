using System.Collections.Immutable;

namespace Caravel.Abstractions;

public interface INode
{
    /// <summary>
    /// Get the neighbors <see cref="IEdge"/>.
    /// </summary>
    /// <returns>The neighbors</returns>
    public ImmutableHashSet<IEdge> GetEdges();

    /// <summary>
    /// Method called each time a <see cref="INode"/> is visited by <see cref="Caravel"/>.
    /// </summary>
    /// <param name="journey">The current <see cref="IJourney"/>.</param>
    /// <param name="cancellationToken">The CancellationToken.</param>
    /// <returns>A Task.</returns>
    public Task OnNodeVisitedAsync(IJourney journey, CancellationToken cancellationToken);
}
