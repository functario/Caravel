using Caravel.Abstractions;
using Caravel.Core.Configurations;

namespace Caravel.Core.UnitTests.Tests.Navigation.Atomic;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureFormatGraph)]
public class AtomicHasEmptySequenceDiagram
{
    [Fact(DisplayName = $"When dummy {nameof(IJourneyLeg)} handlers are used")]
    public async Task Test1()
    {
        // Arrange
        var noSequenceRecord = JourneyLegConfigurationOptions.None;
        // csharpier-ignore
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .Done()
            .Build(noSequenceRecord);

        // Act
        var sut = await journey.GotoAsync<Node1>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }
}
