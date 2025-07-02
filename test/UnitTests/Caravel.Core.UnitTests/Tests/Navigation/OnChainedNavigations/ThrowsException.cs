using AwesomeAssertions;
using Caravel.Abstractions.Exceptions;

namespace Caravel.Core.UnitTests.Tests.Navigation.OnChainedNavigations;

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
            .WithEdge<NodeSpy2>()
            .Done()
            .AddNode<NodeSpy2>()
            .WithEdge<NodeSpy3>(1, "Node3-1")
            .WithEdge<NodeSpy3>(1, "Node3-2")
            .WithEdge<NodeSpy3>(2, "Node3-3")
            .WithEdge<NodeSpy4>(2, "Node4-1")
            .WithEdge<NodeSpy4>(1, "Node4-2")
            .WithEdge<NodeSpy4>(2, "Node4-3")
            .Done()
            .AddNode<NodeSpy3>()
            .Done()
            .AddNode<NodeSpy4>()
            .Done();

        var journey = builder.Build();

        // Act
        var sut = async () => await journey.GotoAsync<NodeSpy4>();

        // Assert
        await sut.Should()
            .ThrowExactlyAsync<DuplicatedEdgesException>()
            .WithMessage(
                "Multiple IEdge with the same origin,"
                    + " neighbor and weight detected "
                    + "('Caravel.Tests.Fixtures.NodeSpy2"
                    + " -->|1| Caravel.Tests.Fixtures.NodeSpy3;"
                    + "Caravel.Tests.Fixtures.NodeSpy2 "
                    + "-->|2| Caravel.Tests.Fixtures.NodeSpy4')."
            );
    }
}
