using AwesomeAssertions;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Core.UnitTests.Tests.Navigation.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureNavigation)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "Test nomenclature."
)]
public class ThrowsException
{
    [Fact(DisplayName = $"When duplicated edges")]
    public async Task Test1()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>(1, "Node3-1")
            .WithEdge<Node3>(1, "Node3-2")
            .WithEdge<Node3>(2, "Node3-3")
            .WithEdge<Node4>(2, "Node4-1")
            .WithEdge<Node4>(1, "Node4-2")
            .WithEdge<Node4>(2, "Node4-3")
            .Done()
            .AddNode<Node3>()
            .Done()
            .AddNode<Node4>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = async () => await journey.GotoAsync<Node4>();

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<DuplicatedEdgesException>()
            .WithMessage(
                "Multiple IEdge with the same origin,"
                    + " neighbor and weight detected "
                    + "('Caravel.Tests.Fixtures.Node2"
                    + " -->|1| Caravel.Tests.Fixtures.Node3;"
                    + "Caravel.Tests.Fixtures.Node2 "
                    + "-->|2| Caravel.Tests.Fixtures.Node4')."
            );
    }
}
