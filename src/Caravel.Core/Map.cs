using Caravel.Abstractions;

namespace Caravel.Core;
public class Map : IMap
{
    private readonly Lazy<HashSet<INode>> _nodes;
    private readonly Lazy<IGraph> _graph;

    public Map(Lazy<HashSet<INode>> nodes, Lazy<IGraph> graph)
    {
        _nodes = nodes;
        _graph = graph;
    }

    public HashSet<INode> Nodes => _nodes.Value;
}
