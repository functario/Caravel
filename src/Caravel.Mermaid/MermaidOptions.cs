namespace Caravel.Mermaid;

public sealed record MermaidOptions(
    bool DisplayDescription = false,
    bool DisplayQuadrant = false,
    MermaidGraphDirections GraphDirection = default
)
{
    public static MermaidOptions Default() => new();
}
