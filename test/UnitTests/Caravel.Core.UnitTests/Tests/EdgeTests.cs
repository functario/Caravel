using Caravel.Tests.Fixtures;

namespace Caravel.Core.UnitTests.Tests;

[Trait(TestType, Unit)]
public class EdgeTests
{
    [Theory(DisplayName = $"Edge.ToString() displays like 'Origin.Name =>Weight Neighbor.Name'")]
    [CaravelData]
    public void Test1(Node origin, Node neighbor, int weight)
    {
        // Arrange
        var edge = new Edge(origin, neighbor, weight);

        // Act
        var sut = edge.ToString();

        // Assert
        sut.Should().Be($"{origin?.Name} =>{weight} {neighbor?.Name}");
    }
}
