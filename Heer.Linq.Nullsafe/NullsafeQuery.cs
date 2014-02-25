using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Heer.Linq.Nullsafe
{
    /// <summary>
    /// Proxy for nullsafe query.
    /// </summary>
    internal class NullsafeQuery : IQueryable
    {
        private readonly Type elementType;
        private readonly Expression expression;
        private readonly IQueryProvider provider;
        private readonly Lazy<IEnumerable> enumerable;

        public NullsafeQuery(IQueryable innerQuery)
        {
            if (innerQuery == null)
                throw new ArgumentNullException("innerQuery");

            elementType = innerQuery.ElementType;
            expression = innerQuery.Expression;

            provider = new NullsafeQueryProvider(innerQuery.Provider);

            enumerable = new Lazy<IEnumerable>(() =>
                Provider.Execute<IEnumerable>(Expression));
        }

        public Type ElementType { get { return elementType; } }
        public Expression Expression { get { return expression; } }
        public IQueryProvider Provider { get { return provider; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return enumerable.Value.GetEnumerator();
        }
    }

    /// <summary>
    /// Proxy for nullsafe query.
    /// </summary>
    internal class NullsafeOrderedQuery : NullsafeQuery, IOrderedQueryable
    {
        public NullsafeOrderedQuery(IOrderedQueryable innerQuery)
            : base(innerQuery)
        {
            if (innerQuery == null)
                throw new ArgumentNullException("innerQuery");
        }
    }

    /// <summary>
    /// Proxy for nullsafe query.
    /// </summary>
    internal class NullsafeQuery<T> : NullsafeQuery, IQueryable<T>
    {
        private readonly Lazy<IEnumerable<T>> enumerable;

        public NullsafeQuery(IQueryable<T> innerQuery)
            : base(innerQuery)
        {
            if (innerQuery == null)
                throw new ArgumentNullException("innerQuery");

            enumerable = new Lazy<IEnumerable<T>>(() =>
                Provider.Execute<IEnumerable<T>>(Expression));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return enumerable.Value.GetEnumerator();
        }
    }

    /// <summary>
    /// Proxy for nullsafe query.
    /// </summary>
    internal class NullsafeOrderedQuery<T> : NullsafeQuery<T>, IOrderedQueryable<T>
    {
        public NullsafeOrderedQuery(IOrderedQueryable<T> innerQuery)
            : base(innerQuery)
        {
            if (innerQuery == null)
                throw new ArgumentNullException("innerQuery");
        }
    }
}
