namespace Caravel.Abstractions;

public interface IExcludedWaypoints : IEnumerable<Type>
{
    int Count { get; }
    Type this[int index] { get; }

    public static IExcludedWaypoints Empty() => EmptyIExcludedWaypoints.Instance;

    private sealed class EmptyIExcludedWaypoints : IExcludedWaypoints
    {
        public static readonly EmptyIExcludedWaypoints Instance = new();

        private EmptyIExcludedWaypoints() { }

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
