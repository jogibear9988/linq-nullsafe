using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Heer.Linq.Nullsafe
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class RewriteQuery : IQueryable
    {
        private readonly Type elementType;
        private readonly Expression expression;
        private readonly IQueryProvider provider;
        private readonly Lazy<IEnumerable> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteQuery(IQueryable query, ExpressionVisitor rewriter)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (rewriter == null)
                throw new ArgumentNullException("rewriter");

            elementType = query.ElementType;
            expression = query.Expression;

            // replace query provider
            provider = new RewriteQueryProvider(query.Provider, rewriter);

            enumerable = new Lazy<IEnumerable>(() =>
                Provider.Execute<IEnumerable>(Expression));
        }

        /// <inheritdoc />
        public Type ElementType { get { return elementType; } }
        /// <inheritdoc />
        public Expression Expression { get { return expression; } }
        /// <inheritdoc />
        public IQueryProvider Provider { get { return provider; } }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return enumerable.Value.GetEnumerator();
        }
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class RewriteOrderedQuery : RewriteQuery, IOrderedQueryable
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteOrderedQuery(IOrderedQueryable query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
        }
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class RewriteQuery<T> : RewriteQuery, IQueryable<T>
    {
        private readonly Lazy<IEnumerable<T>> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteQuery(IQueryable<T> query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
            enumerable = new Lazy<IEnumerable<T>>(() =>
                Provider.Execute<IEnumerable<T>>(Expression));
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator()
        {
            return enumerable.Value.GetEnumerator();
        }
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class RewriteOrderedQuery<T> : RewriteQuery<T>, IOrderedQueryable<T>
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteOrderedQuery(IOrderedQueryable<T> query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
        }
    }
}
