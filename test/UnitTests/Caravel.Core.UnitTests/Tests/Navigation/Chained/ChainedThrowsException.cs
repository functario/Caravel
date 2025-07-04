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
public class ChainedThrowsException
{
    [Fact(DisplayName = $"When duplicated edges")]
    public async Task Test1()
    {
        // Arrange
        // NOTE:
        // Since ImmutableHashSet eliminates duplicates
        // We need to add a description to add a difference
        // (expect to be a diff in delegate in real usage).

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

    [Fact(DisplayName = $"When no route found between waypoints")]
    public async Task Tes2()
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
        Waypoints waypoints = [typeof(Node4)];
        // Act
        // csharpier-ignore
        var sut = async () => await journey
            .GotoAsync<Node3>()
            .GotoAsync<Node5>(waypoints);

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<RouteNotFoundException>()
            .WithMessage(
                "No IRoute found between "
                    + "origin INode 'Caravel.Tests.Fixtures.Node3' "
                    + "and destination INode 'Caravel.Tests.Fixtures.Node4'."
            );
    }

    [Fact(DisplayName = "When origin is also a waypoint")]
    public async Task Test4()
    {
        // Arrange
        // csharpier-ignore-start
        Waypoints waypoints = [typeof(Node2), typeof(Node1)];
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .WithEdge<Node1>()
            .Done()
            .AddNode<Node3>()
            .Done()
            .Build();

        // Act
        var sut = async () => await journey
        .GotoAsync<Node2>() // Set Node2 as origin
        .GotoAsync<Node3>(waypoints);

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<InvalidRouteException>()
            .WithMessage(
                "Invalid IRoute detected "
                    + "with reason 'OriginIsAlsoWaypoint' "
                    + "(origin: 'Caravel.Tests.Fixtures.Node2', "
                    + "destination: 'Caravel.Tests.Fixtures.Node3')."
            );

        // csharpier-ignore-end
    }

    [Fact(DisplayName = "When destination is also a waypoint")]
    public async Task Test5()
    {
        // Arrange
        // csharpier-ignore-start
        Waypoints waypoints = [typeof(Node1), typeof(Node3)];
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .WithEdge<Node1>()
            .Done()
            .AddNode<Node3>()
            .WithEdge<Node2>()
            .Done()
            .Build();

        // Act
        var sut = async () => await journey
            .GotoAsync<Node2>()
            .GotoAsync<Node3>(waypoints); // waypoints contains Node3

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<InvalidRouteException>()
            .WithMessage(
                "Invalid IRoute detected "
                    + "with reason 'DestinationIsAlsoWaypoint' "
                    + "(origin: 'Caravel.Tests.Fixtures.Node2', "
                    + "destination: 'Caravel.Tests.Fixtures.Node3')."
            );

        // csharpier-ignore-end
    }
}
