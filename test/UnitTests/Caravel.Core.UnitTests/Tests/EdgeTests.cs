//using Caravel.Abstractions;
//using Caravel.Tests.Fixtures.GraphsData;
//using Caravel.Tests.Fixtures.GraphsData.Nodes;

//namespace Caravel.Core.UnitTests.Tests;

//[Trait(TestType, Unit)]
//public class EdgeTests
//{
//    [Fact(DisplayName = $"Edge.ToString() displays like 'Origin.Name =>Weight Neighbor.Name'")]
//    public void Test1()
//    {
//        // Arrange

//        // TODO: Resolve Lazy Node (by reflection or injection).
//        // ou bien revoir l'implémentation et utiliser des objects generique avec la func (voir INodeFunc).

//        var map = new MapA();
//        var origin = new NodeA(map);
//        var neighbor = new NodeB(map);
//        var edge = new Edge(origin, neighbor, (_) => Task.FromResult<INode>(neighbor));

//        // Act
//        var sut = edge.ToString();

//        // Assert
//        sut.Should().Be($"{origin?.Name} =>{edge.Weight} {neighbor?.Name}");
//    }
//}

