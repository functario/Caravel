namespace Caravel.Mermaid;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GridPositionAttribute : Attribute
{
    public int Row { get; }
    public int Column { get; }

    public GridPositionAttribute(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
