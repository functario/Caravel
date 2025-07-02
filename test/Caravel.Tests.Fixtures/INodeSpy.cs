using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures;

public interface INodeSpy : INode
{
    ImmutableHashSet<IEdge> InternalEdges { get; }
}
