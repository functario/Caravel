using Caravel.Tests.Fixtures.GraphsData.Nodes;

namespace Caravel.Core.UnitTests.Tests;

[Trait(TestType, Unit)]
public class EdgeTests
{
    [Fact(DisplayName = $"Display the Edge link like 'Origin.Name =>Weight Neighbor.Name'")]
    public void Test1()
    {
        // Arrange
        var origin = typeof(NodeA);
        var neighbor = typeof(NodeB);
        var edge = new Edge(origin, neighbor, (_) => Task.CompletedTask);

        // Act
        var sut = edge.ToString();

        // Assert
        sut.Should().Be($"{origin?.Name} *{edge.Weight}=> {neighbor?.Name}");
    }
}
