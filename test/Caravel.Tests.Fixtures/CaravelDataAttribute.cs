using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Caravel.Tests.Fixtures;

public sealed class CaravelDataAttribute : AutoDataAttribute
{
    //private static int s_nodeNameCounter;

    public CaravelDataAttribute()
        : base(CreateFixture) { }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Globalization",
        "CA1305:Specify IFormatProvider",
        Justification = "Unnecessary"
    )]
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        //fixture.
        //fixture.Customize<INode>(x =>
        //    x.FromFactory(() => new Node((++s_nodeNameCounter).ToString()))
        //);

        //fixture.Customize<Graph_3_Nodes_NoWeight>(x =>
        //    x.FromFactory(() => new Graph_3_Nodes_NoWeight())
        //);

        return fixture;
    }
}
