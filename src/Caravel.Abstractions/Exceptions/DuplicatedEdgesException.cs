namespace Caravel.Abstractions.Exceptions;

/// <summary>
/// Exception throws when <see cref="IEdge"/> have the same origin, neighbor and weight.
/// </summary>
public class DuplicatedEdgesException : CaravelException
{
    private static string DefaultMessage(ICollection<IEdge> edges) =>
        $"Multiple {nameof(IEdge)} with the same origin, neighbor and weight detected "
        + $"('{string.Join(';',
            [.. edges.Select(x => $"{x.Origin} -->|{x.Weight}| {x.Neighbor}").Order()])}').";

    public DuplicatedEdgesException(ICollection<IEdge> edges)
        : base(DefaultMessage(edges)) { }
}
