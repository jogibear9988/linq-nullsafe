using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Heer.Linq.Nullsafe
{
    /// <summary>
    /// Expression visitor for making member access nullsafe.
    /// </summary>
    /// <remarks>
    /// Use <see cref="NullsafeQueryBuilder" /> to make a query nullsafe.
    /// </remarks>
    public class NullsafeQueryRewriter : ExpressionVisitor
    {
        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null || node.Expression == null)
                return node;

            var fallback = fallbackOfType(node.Type);

            // check both, expression's value and expression's member's value, if not default
            if (fallback.NodeType != ExpressionType.Default)
            {
                return Expression.Condition(
                    Expression.OrElse(
                        Expression.Equal(Visit(node.Expression), Expression.Default(node.Expression.Type)),
                        Expression.Equal(node, Expression.Default(node.Type))),
                    fallback,
                    node);
            }

            // just check expression's value...
            return Expression.Condition(
                Expression.Equal(Visit(node.Expression), Expression.Default(node.Expression.Type)),
                fallback,
                node);
        }

        private static Expression fallbackOfType(Type type)
        {
            // default value for IEnumerable<T> and ICollection<T>
            if (type.IsConstructedGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                if (typeDefinition == typeof(IEnumerable<>) || typeDefinition == typeof(ICollection<>))
                {
                    var typeArguments = type.GenericTypeArguments;
                    return Expression.Convert(
                        Expression.New(typeof(List<>).MakeGenericType(typeArguments)),
                        type);
                }
            }

            // default value
            return Expression.Default(type);
        }
    }
}
