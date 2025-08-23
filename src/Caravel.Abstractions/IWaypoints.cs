namespace Caravel.Abstractions;

public interface IWaypoints : IEnumerable<Type>
{
    int Count { get; }

    // Indexer property
    Type this[int index] { get; }

    public static IWaypoints Empty() => EmptyIWaypoints.Instance;

    private sealed class EmptyIWaypoints : IWaypoints
    {
        public static readonly EmptyIWaypoints Instance = new();

        private EmptyIWaypoints() { }

        public int Count => 0;

        public Type this[int index] => throw new ArgumentOutOfRangeException(nameof(index));

        public static IEnumerator<Type> GetEnumerator() => Enumerable.Empty<Type>().GetEnumerator();

        IEnumerator<Type> IEnumerable<Type>.GetEnumerator() => GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
