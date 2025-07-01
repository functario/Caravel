using System.Collections.Immutable;

namespace Caravel.Abstractions;

public interface INode
{
    public ImmutableHashSet<IEdge> GetEdges();

    // TODO: Document that OnNodeOpenedAsync can be used to log, throw exception or to change a StateMachine.
    // In the case of the StateMachine, it can be used to dynamically modify the Node.GetEdges().
    // OnNodeOpenedAsync is always executed before Node.GetEdges().

    /// <summary>
    /// Method called each time a <see cref="INode"/> is opened.
    /// </summary>
    /// <param name="journey">The current <see cref="IJourney"/>.</param>
    /// <param name="cancellationToken">The CancellationToken.</param>
    /// <returns>A Task.</returns>
    public Task OnNodeOpenedAsync(IJourney journey, CancellationToken cancellationToken);
}
