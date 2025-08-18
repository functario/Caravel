using System.Collections;
using System.Runtime.CompilerServices;
using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

[CollectionBuilder(typeof(EmptyWaypoints), nameof(Create))]
internal sealed class EmptyWaypoints : IWaypoints
{
    public static EmptyWaypoints Create() => new();

    private readonly Type[] _values;

    public EmptyWaypoints()
    {
        _values = [];
    }

    public Type this[int index] => _values[index];
    public int Count => _values.Length;

    public IEnumerator<Type> GetEnumerator() => _values.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
