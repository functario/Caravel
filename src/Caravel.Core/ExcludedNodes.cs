using System.Collections;
using System.Runtime.CompilerServices;
using Caravel.Abstractions;

namespace Caravel.Core;

[CollectionBuilder(typeof(ExcludedNodes), nameof(Create))]
public sealed class ExcludedNodes : IExcludedNodes
{
    public static ExcludedNodes Empty() => new([]);

    public static ExcludedNodes Create(ReadOnlySpan<Type> values) => new(values);

    private readonly Type[] _values;

    public ExcludedNodes(ReadOnlySpan<Type> values)
    {
        _values = values.ToArray();
    }

    public Type this[int index] => _values[index];
    public int Count => _values.Length;

    public IEnumerator<Type> GetEnumerator() => _values.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
