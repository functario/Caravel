using AutoFixture;
using AutoFixture.Xunit3;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Tests.Fixtures.GraphsData;

namespace Caravel.Tests.Fixtures;

public sealed class CaravelDataAttribute : AutoDataAttribute
{
    private static int s_nodeNameCounter;

    public CaravelDataAttribute()
        : base(CreateFixture) { }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Globalization",
        "CA1305:Specify IFormatProvider",
        Justification = "Unnecessary"
    )]
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<INode>(x =>
            x.FromFactory(() => new Node((++s_nodeNameCounter).ToString()))
        );

        fixture.Customize<Graph_3_Nodes_NoWeight>(x =>
            x.FromFactory(() => new Graph_3_Nodes_NoWeight())
        );

        return fixture;
    }
}
