namespace Caravel.Mermaid;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class QuadrantAttribute : Attribute
{
    public int Row { get; }
    public int Column { get; }

    public QuadrantAttribute(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
