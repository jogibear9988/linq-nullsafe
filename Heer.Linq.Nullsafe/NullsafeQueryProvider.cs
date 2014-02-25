using System;
using System.Linq;
using System.Linq.Expressions;

namespace Heer.Linq.Nullsafe
{
    /// <summary>
    /// Proxy for nullsafe query provider.
    /// </summary>
    internal class NullsafeQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider innerProvider;

        public NullsafeQueryProvider(IQueryProvider innerProvider)
        {
            if (innerProvider == null)
                throw new ArgumentNullException("innerProvider");

            this.innerProvider = innerProvider;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for nullsafe query chaining
            return innerProvider.CreateQuery<TElement>(expression).ToNullsafe();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            // create query and make proxy again for nullsafe query chaining
            return innerProvider.CreateQuery(expression).ToNullsafe();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rewriter = new NullsafeQueryRewriter();
            expression = rewriter.Visit(expression);

            // execute query with rewritten expression
            return innerProvider.Execute<TResult>(expression);
        }

        public object Execute(Expression expression)
        {
            var rewriter = new NullsafeQueryRewriter();
            expression = rewriter.Visit(expression);

            // execute query with rewritten expression
            return innerProvider.Execute(expression);
        }
    }
}
