using AwesomeAssertions;
using Caravel.Abstractions;

namespace Caravel.Core.UnitTests.Tests.Abstractions;

[Trait(TestType, Unit)]
public sealed class IExcludedNodesTests
{
    [Fact(DisplayName = "Empty() is empty")]
    public void Test1()
    {
        // Act
        var sut = IExcludedWaypoints.Empty();

        // Assert
        sut.Should().BeEmpty();
        sut.Count.Should().Be(0);
    }

    [Fact(DisplayName = "Throws ArgumentOutOfRangeException when using indexer")]
    public void Test2()
    {
        // Arrange
        var empty = IExcludedWaypoints.Empty();

        // Act
        var sut = () => empty[0];

        // Assert
        sut.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Has an IEnumerator<Type>")]
    public void Test3()
    {
        // Arrange
        var empty = (IEnumerable<Type>)IExcludedWaypoints.Empty();

        // Act
        var sut = empty.GetEnumerator();

        // Assert
        sut.Should().BeAssignableTo<IEnumerator<Type>>();
        sut.Should().NotBeNull();
    }
}
