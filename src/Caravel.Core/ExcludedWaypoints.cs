using System.Collections;
using System.Runtime.CompilerServices;
using Caravel.Abstractions;

namespace Caravel.Core;

[CollectionBuilder(typeof(ExcludedWaypoints), nameof(Create))]
public sealed class ExcludedWaypoints : IExcludedWaypoints
{
    public static ExcludedWaypoints Create(ReadOnlySpan<Type> values) => new(values);

    private readonly Type[] _values;

    public ExcludedWaypoints(ReadOnlySpan<Type> values)
    {
        _values = values.ToArray();
    }

    public Type this[int index] => _values[index];
    public int Count => _values.Length;

    public IEnumerator<Type> GetEnumerator() => _values.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
