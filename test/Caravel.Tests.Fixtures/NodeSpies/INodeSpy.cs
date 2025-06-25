using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures.NodeSpies;
public interface INodeSpy : IAuditableNode
{
    ImmutableHashSet<IEdge> InternalEdges { get; }
}
