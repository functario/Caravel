using AwesomeAssertions;
using Caravel.Abstractions;

namespace Caravel.Core.UnitTests.Tests.Abstractions;

[Trait(TestType, Unit)]
public sealed class IWaypointsTests
{
    [Fact(DisplayName = "Empty() is empty")]
    public void Test1()
    {
        // Act
        var sut = IWaypoints.Empty();

        // Assert
        sut.Should().BeEmpty();
        sut.Count.Should().Be(0);
    }

    [Fact(DisplayName = "Throws ArgumentOutOfRangeException when using indexer")]
    public void Test2()
    {
        // Arrange
        var emptyWaypoints = IWaypoints.Empty();

        // Act
        var sut = () => emptyWaypoints[0];

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
