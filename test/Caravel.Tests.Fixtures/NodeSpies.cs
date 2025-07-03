using System.Collections.Immutable;
using Caravel.Abstractions;
using Caravel.Mermaid;

namespace Caravel.Tests.Fixtures;

[Quadrant(1, 1)]
public sealed class Node1 : NodeSpyBase
{
    public Node1(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(2, 1)]
public sealed class Node2 : NodeSpyBase
{
    public Node2(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(2, 2)]
public sealed class Node3 : NodeSpyBase
{
    public Node3(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(3, 1)]
public sealed class Node4 : NodeSpyBase
{
    public Node4(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(3, 2)]
public sealed class Node5 : NodeSpyBase
{
    public Node5(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(3, 3)]
public sealed class Node6 : NodeSpyBase
{
    public Node6(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(3, 4)]
public sealed class Node7 : NodeSpyBase
{
    public Node7(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 1)]
public sealed class Node8 : NodeSpyBase
{
    public Node8(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 2)]
public sealed class Node9 : NodeSpyBase
{
    public Node9(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 3)]
public sealed class Node10 : NodeSpyBase
{
    public Node10(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 4)]
public sealed class Node11 : NodeSpyBase
{
    public Node11(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 5)]
public sealed class Node12 : NodeSpyBase
{
    public Node12(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 6)]
public sealed class Node13 : NodeSpyBase
{
    public Node13(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 7)]
public sealed class Node14 : NodeSpyBase
{
    public Node14(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}

[Quadrant(4, 8)]
public sealed class Node15 : NodeSpyBase
{
    public Node15(ImmutableHashSet<IEdge> edges, bool existValue = true)
        : base(edges, existValue) { }
}
