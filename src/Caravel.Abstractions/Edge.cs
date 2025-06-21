namespace Caravel.Abstractions;
public record Edge(Node Origin, Node Neighbor, int Weight = 0)
{
    public override string ToString() => $"{Origin.Name} =>{Weight} {Neighbor.Name}";
}
