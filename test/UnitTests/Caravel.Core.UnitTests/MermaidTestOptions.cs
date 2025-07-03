using Caravel.Mermaid;

namespace Caravel.Core.UnitTests;

internal static class MermaidTestOptions
{
    public static MermaidOptions WithDescription = new() { DisplayDescription = true };
    public static MermaidOptions WithDescriptionAndQuadrant = new()
    {
        DisplayDescription = true,
        DisplayQuadrant = true,
    };
    public static MermaidOptions WithQuadrant = new() { DisplayQuadrant = true };
}
