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
            _map.CreateEdge<NodeA>(this, (ct) => Task.FromResult(_map.NodeA))
        }.ToFrozenSet();
    }
}
