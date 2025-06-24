using System.Collections;
using System.Runtime.CompilerServices;
using Caravel.Abstractions;

namespace Caravel.Core;

[CollectionBuilder(typeof(Waypoints), nameof(Create))]
public sealed class Waypoints : IWaypoints
{
    public static Waypoints Empty() => new([]);
    public static Waypoints Create(ReadOnlySpan<Type> values) => new(values);

    private readonly Type[] _values;
    public Waypoints(ReadOnlySpan<Type> values)
    {
        _values = values.ToArray();
    }
    public Type this[int index] => _values[index];
    public int Count => _values.Length;

    public IEnumerator<Type> GetEnumerator() => _values.AsEnumerable().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

