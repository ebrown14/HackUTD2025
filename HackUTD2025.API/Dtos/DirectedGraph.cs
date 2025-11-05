namespace HackUTD2025.API.Dtos;

public sealed class DirectedGraph
{
    private readonly Dictionary<string, List<NeighborDto>> _adj =
        new(StringComparer.OrdinalIgnoreCase);

    public DirectedGraph(IEnumerable<NodeDto> nodes, IEnumerable<EdgeDto> edges)
    {
        foreach (var n in nodes) _adj.TryAdd(n.id, new());
        foreach (var e in edges)
        {
            _adj.TryAdd(e.from, new());
            _adj[e.from].Add(new(e.to, TimeSpan.FromMinutes(e.travel_time_minutes)));
        }
    }

    public IEnumerable<NeighborDto> Neighbors(string nodeId)
    {
        if (_adj.TryGetValue(nodeId, out var list))
            return list;
        return [];
    }
}

public record NeighborDto(
    string to,
    TimeSpan cost);