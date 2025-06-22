using System.Collections.Frozen;
using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Tests.Fixtures.GraphsData.Nodes;

public sealed class NodeB : INode
{
    private readonly MapA _map;

    public NodeB(MapA map)
    {
        _map = map;
    }

    public string Name => nameof(NodeA);

    public FrozenSet<IEdge> GetEdges()
    {
        return new List<IEdge>()
        {
            new Edge(this, _map.NodeA, (ct) => Task.FromResult<INode>(_map.NodeA)),
        }.ToFrozenSet();
    }
}
