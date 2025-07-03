using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Core;

/// <summary>
/// <see cref="IRoute"/> created from a <see cref="IJourney.DoAsync"/> navigation.
/// </summary>
/// <remarks>Only to use internally.</remarks>
internal sealed class DoRoute : IRoute
{
    public ImmutableList<IEdge> Edges { get; }

    public DoRoute(ImmutableList<IEdge> edges)
    {
        Edges = edges;
    }
}
