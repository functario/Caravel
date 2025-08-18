namespace Caravel.Mermaid;

public sealed record MermaidOptions(
    bool DisplayDescription = false,
    bool DisplayGridPosition = false,
    MermaidGraphDirections GraphDirection = default
)
{
    public static MermaidOptions Default() => new();
}
