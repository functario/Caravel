using Caravel.Abstractions;

namespace Caravel.Tests.Fixtures;

public sealed class Map
{
    private readonly ICollection<INode> _nodes;

    public Map(ICollection<INode> nodes)
    {
        _nodes = nodes;
    }

    public Node1 NodeSpy1 => GetNode<Node1>();
    public Node2 NodeSpy2 => GetNode<Node2>();
    public Node3 NodeSpy3 => GetNode<Node3>();
    public Node4 NodeSpy4 => GetNode<Node4>();
    public Node5 NodeSpy5 => GetNode<Node5>();
    public Node6 NodeSpy6 => GetNode<Node6>();
    public Node7 NodeSpy7 => GetNode<Node7>();
    public Node8 NodeSpy8 => GetNode<Node8>();
    public Node9 NodeSpy9 => GetNode<Node9>();
    public Node10 NodeSpy10 => GetNode<Node10>();
    public Node11 NodeSpy11 => GetNode<Node11>();
    public Node12 NodeSpy12 => GetNode<Node12>();
    public Node13 NodeSpy13 => GetNode<Node13>();
    public Node14 NodeSpy14 => GetNode<Node14>();
    public Node15 NodeSpy15 => GetNode<Node15>();

    private T GetNode<T>()
        where T : INode => (T)_nodes.Where(x => x.GetType() == typeof(T)).Single();
}
