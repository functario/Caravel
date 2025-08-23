using Caravel.Abstractions;
using Caravel.Core.Configurations;

namespace Caravel.Core.UnitTests.Tests.Navigation.Chained;

[Trait(TestType, Unit)]
[Trait(Feature, FeatureFormatGraph)]
public class ChainedHasEmptySequenceDiagram
{
    [Fact(DisplayName = $"When dummy {nameof(IJourneyLeg)} handlers are used")]
    public async Task Test1()
    {
        // Arrange
        var noSequenceRecord = JourneyLegConfigurationOptions.None;
        var journey = new JourneyBuilder()
            .AddNode<Node1>()
            .WithEdge<Node2>()
            .Done()
            .AddNode<Node2>()
            .WithEdge<Node3>()
            .Done()
            .AddNode<Node3>()
            .Done()
            .Build(noSequenceRecord);
        // Act
        // csharpier-ignore
        var sut = await journey
            .GotoAsync<Node2>()
            .GotoAsync<Node2>() // self reference
            .GotoAsync<Node3>();

        // Assert
        var result = await sut.ToMermaidSequenceDiagramMarkdownAsync(WithGridPosition);
        await result.VerifyMermaidMarkdownAsync();
    }
}
