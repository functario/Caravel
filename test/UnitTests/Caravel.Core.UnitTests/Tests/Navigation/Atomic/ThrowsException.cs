using AwesomeAssertions;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

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
            .WithEdge<Node2>(1, "Node2-1")
            .WithEdge<Node2>(1, "Node2-2")
            .WithEdge<Node2>(2, "Node2-3")
            .WithEdge<Node3>(2, "Node3-1")
            .WithEdge<Node3>(1, "Node3-2")
            .WithEdge<Node3>(2, "Node3-3")
            .Done()
            .AddNode<Node2>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = async () => await journey.GotoAsync<Node2>();

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<DuplicatedEdgesException>()
            .WithMessage(
                "Multiple IEdge with the same origin,"
                    + " neighbor and weight detected "
                    + "('Caravel.Tests.Fixtures.Node1"
                    + " -->|1| Caravel.Tests.Fixtures.Node2;"
                    + "Caravel.Tests.Fixtures.Node1 "
                    + "-->|2| Caravel.Tests.Fixtures.Node3')."
            );
    }

    [Fact(DisplayName = $"When no route found between waypoints")]
    public async Task Test2()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node4>()
            .Done()
            .AddNode<Node3>() // dead end
            .Done()
            .AddNode<Node4>()
            .WithEdge<Node5>()
            .Done()
            .AddNode<Node5>()
            .Done();

        var journey = builder.Build();
        Waypoints waypoints = [typeof(Node3)];

        // Act
        var sut = async () => await journey.GotoAsync<Node5>(waypoints);

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<RouteNotFoundException>()
            .WithMessage(
                "No IRoute found between "
                    + "origin INode 'Caravel.Tests.Fixtures.Node3' "
                    + "and destination INode 'Caravel.Tests.Fixtures.Node5'."
            );
    }

    [Fact(DisplayName = "When origin self references itself in edge")]
    public async Task Test3()
    {
        // Arrange
        // csharpier-ignore
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node1>()
            .Done()
            .Build();

        // Act
        var sut = async () => await journey.GotoAsync<Node1>();

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<InvalidEdgeException>()
            .WithMessage("Invalid IEdge detected with reason 'NodeHasEdgeToItself'.");
    }
}
