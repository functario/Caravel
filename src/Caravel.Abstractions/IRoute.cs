using System.Collections.Immutable;

namespace Caravel.Abstractions;
public interface IRoute
{
    ImmutableList<IEdge> Edges { get; }
    public string[] GetPath();
}
