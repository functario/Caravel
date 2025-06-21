using System.Collections.Frozen;
using Caravel.Abstractions;

namespace Caravel.Core;

public record Node(string Name, FrozenSet<IEdge> Edges) : INode
{
}
