using Caravel.Mermaid;

namespace Caravel.Core.UnitTests;

internal static class MermaidTestOptions
{
    public static MermaidOptions WithDescription = new() { DisplayDescription = true };
    public static MermaidOptions WithDescriptionAndGridPosition = new()
    {
        DisplayDescription = true,
        DisplayGridPosition = true,
    };
    public static MermaidOptions WithGridPosition = new() { DisplayGridPosition = true };
}
