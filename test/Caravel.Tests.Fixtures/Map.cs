using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures;

public sealed class Map
{
    private readonly ICollection<INode> _nodes;

    public Map(ICollection<INode> nodes)
    {
        _nodes = nodes;
    }

    public NodeSpy1 NodeSpy1 => GetNode<NodeSpy1>();
    public NodeSpy2 NodeSpy2 => GetNode<NodeSpy2>();
    public NodeSpy3 NodeSpy3 => GetNode<NodeSpy3>();
    public NodeSpy4 NodeSpy4 => GetNode<NodeSpy4>();
    public NodeSpy5 NodeSpy5 => GetNode<NodeSpy5>();
    public NodeSpy6 NodeSpy6 => GetNode<NodeSpy6>();
    public NodeSpy7 NodeSpy7 => GetNode<NodeSpy7>();
    public NodeSpy8 NodeSpy8 => GetNode<NodeSpy8>();
    public NodeSpy9 NodeSpy9 => GetNode<NodeSpy9>();
    public NodeSpy10 NodeSpy10 => GetNode<NodeSpy10>();
    public NodeSpy11 NodeSpy11 => GetNode<NodeSpy11>();
    public NodeSpy12 NodeSpy12 => GetNode<NodeSpy12>();
    public NodeSpy13 NodeSpy13 => GetNode<NodeSpy13>();
    public NodeSpy14 NodeSpy14 => GetNode<NodeSpy14>();
    public NodeSpy15 NodeSpy15 => GetNode<NodeSpy15>();

    private T GetNode<T>()
        where T : INode => (T)_nodes.Where(x => x.GetType() == typeof(T)).Single();
}
