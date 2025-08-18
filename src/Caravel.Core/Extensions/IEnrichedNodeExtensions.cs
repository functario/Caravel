using System.Linq.Expressions;
using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class IEnrichedNodeExtensions
{
    /// <summary>
    /// Try to get the value of a property from an <see cref="EnrichedNode{TNode}"/> instance using reflection.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value to retrieve.</typeparam>
    /// <param name="node">The node to get the property from. Must be an <see cref="EnrichedNode{TNode}"/> instance.</param>
    /// <param name="propertyExpression">An expression that specifies the property to retrieve (e.g., x => x.PropertyName).</param>
    /// <param name="tValue"></param>
    /// <returns>
    /// The value of the specified property if the node is an <see cref="EnrichedNode{TNode}"/> and the property exists;
    /// otherwise, the default value of <typeparamref name="TValue"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="node"/> or <paramref name="propertyExpression"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="propertyExpression"/> is not a valid property access expression.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified property is not found on the node type.
    /// </exception>
    public static bool TryGetPropertyValue<TValue>(
        this INode node,
        Expression<Func<IEnrichedNode<INode>, TValue>> propertyExpression,
        out TValue? tValue
    )
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(propertyExpression);
        tValue = default;
        if (!node.IsAssignableToIEnrichedNode())
        {
            return false;
        }

        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException(
                "Expression must be a property access",
                nameof(propertyExpression)
            );
        }

        var enrichedNodeType = node.GetType();

        var propertyInfo =
            enrichedNodeType.GetProperty(memberExpression.Member.Name)
            ?? throw new InvalidOperationException(
                $"Property '{memberExpression.Member.Name}' not found on type '{enrichedNodeType}'"
            );

        var value = propertyInfo.GetValue(node);
        tValue = value is TValue typedValue ? typedValue : default;
        return true;
    }

    public static bool IsAssignableToIEnrichedNode(this INode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var objType = node.GetType();
        return objType.IsGenericType
            && objType
                .GetInterfaces()
                .Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnrichedNode<>)
                );
    }
}
