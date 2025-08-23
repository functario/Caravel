namespace Caravel.Abstractions;

public interface IExcludedNodes : IEnumerable<Type>
{
    int Count { get; }
    Type this[int index] { get; }

    public static IExcludedNodes Empty() => EmptyIExcludedNodes.Instance;

    private sealed class EmptyIExcludedNodes : IExcludedNodes
    {
        public static readonly EmptyIExcludedNodes Instance = new();

        private EmptyIExcludedNodes() { }

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
