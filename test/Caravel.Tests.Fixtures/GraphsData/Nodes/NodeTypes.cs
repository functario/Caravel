using System.Collections.Immutable;
using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public record Node1 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        throw new NotImplementedException();
    }
}

public record Node2 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        throw new NotImplementedException();
    }
}

public record Node3 : INode
{
    public ImmutableHashSet<IEdge> GetEdges()
    {
        throw new NotImplementedException();
    }
}
