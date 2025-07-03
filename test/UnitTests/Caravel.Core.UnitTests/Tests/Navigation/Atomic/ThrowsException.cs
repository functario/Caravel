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
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>(1, "Node2-1")
            .WithEdge<NodeSpy2>(1, "Node2-2")
            .WithEdge<NodeSpy2>(2, "Node2-3")
            .WithEdge<NodeSpy3>(2, "Node3-1")
            .WithEdge<NodeSpy3>(1, "Node3-2")
            .WithEdge<NodeSpy3>(2, "Node3-3")
            .Done()
            .AddNode<NodeSpy2>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = async () => await journey.GotoAsync<NodeSpy2>();

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<DuplicatedEdgesException>()
            .WithMessage(
                "Multiple IEdge with the same origin,"
                    + " neighbor and weight detected "
                    + "('Caravel.Tests.Fixtures.NodeSpy1"
                    + " -->|1| Caravel.Tests.Fixtures.NodeSpy2;"
                    + "Caravel.Tests.Fixtures.NodeSpy1 "
                    + "-->|2| Caravel.Tests.Fixtures.NodeSpy3')."
            );
    }

    [Fact(DisplayName = $"When no route found between waypoints")]
    public async Task Tes2()
    {
        // Arrange
        var builder = new JourneyBuilder()
            .AddNode<NodeSpy1>()
            .WithEdge<NodeSpy2>()
            .WithEdge<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy4>()
            .Done()
            .AddNode<NodeSpy3>() // dead end
            .Done()
            .AddNode<NodeSpy4>()
            .WithEdge<NodeSpy5>()
            .Done()
            .AddNode<NodeSpy5>()
            .Done();

        var journey = builder.Build();
        Waypoints waypoints = [typeof(NodeSpy3)];

        // Act
        var sut = async () => await journey.GotoAsync<NodeSpy5>(waypoints);

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<RouteNotFoundException>()
            .WithMessage(
                "No IRoute found between "
                    + "origin INode 'Caravel.Tests.Fixtures.NodeSpy3' "
                    + "and destination INode 'Caravel.Tests.Fixtures.NodeSpy5'."
            );
    }
}
