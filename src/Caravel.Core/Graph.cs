using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Caravel.Abstractions;

namespace Caravel.Core;

public class Graph
{
    private readonly FrozenDictionary<INode, List<Edge>> _map = new Dictionary<
        INode,
        List<Edge>
    >().ToFrozenDictionary();

    public Graph(FrozenDictionary<INode, List<Edge>> map)
    {
        _map = map;
    }

    public FrozenDictionary<INode, List<Edge>> Map => _map;

    public ICollection<INode> GetShortestPath(INode start, INode end)
    {
        var distances = new Dictionary<INode, int>();
        var previous = new Dictionary<INode, INode>();
        var queue = new PriorityQueue<INode, int>();

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
        var path = new List<INode>();
        for (var at = end; at != null; at = previous[at])
            path.Insert(0, at);

        return distances[end] == int.MaxValue ? [] : path;
    }

    public ICollection<INode> GetShortestPathWithWayNodes(
        INode start,
        ICollection<INode> wayNodes,
        INode end
    )
    {
        var fullPath = new List<INode>();
        var pathNodes = new List<INode> { start };
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
