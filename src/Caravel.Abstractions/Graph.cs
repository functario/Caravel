using System.Collections.Frozen;

namespace Caravel.Abstractions;

public class Graph
{
    private readonly FrozenDictionary<Node, List<Edge>> _map = new Dictionary<Node, List<Edge>>().ToFrozenDictionary();

    public Graph(FrozenDictionary<Node, List<Edge>> map)
    {
        _map = map;
    }

    public FrozenDictionary<Node, List<Edge>> Map => _map;

    public ICollection<Node> GetShortestPath(Node start, Node end)
    {
        var distances = new Dictionary<Node, int>();
        var previous = new Dictionary<Node, Node?>();
        var queue = new PriorityQueue<Node, int>();

        foreach (var node in _map.Keys)
        {
            distances[node] = int.MaxValue;
            previous[node] = null;
        }

        distances[start] = 0;
        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Equals(end))
                break;

            foreach (var edge in _map[current])
            {
                var neighbor = edge.Neighbor;
                var alt = distances[current] + edge.Weight;

                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor, alt);
                }
            }
        }

        // Reconstruct path
        var path = new List<Node>();
        for (var at = end; at != null; at = previous[at])
            path.Insert(0, at);

        return distances[end] == int.MaxValue ? [] : path;
    }

    public ICollection<Node> GetShortestPathWithWayNodes(
        Node start,
        ICollection<Node> wayNodes,
        Node end
    )
    {
        var fullPath = new List<Node>();
        var pathNodes = new List<Node> { start };
        pathNodes.AddRange(wayNodes);
        pathNodes.Add(end);

        for (var i = 0; i < pathNodes.Count - 1; i++)
        {
            var segment = GetShortestPath(pathNodes[i], pathNodes[i + 1]).ToList();
            if (segment.Count == 0)
                return []; // No path

            if (i > 0)
                segment.RemoveAt(0); // avoid duplicate

            fullPath.AddRange(segment);
        }

        return fullPath;
    }
}
