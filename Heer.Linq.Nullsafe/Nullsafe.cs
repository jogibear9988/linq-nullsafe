using System;
using System.Linq;

namespace Heer.Linq.Nullsafe
{
    /// <summary>
    /// Makes the world a better world.
    /// </summary>
    public static class Nullsafe
    {
        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable ToNullsafe(this IQueryable value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var ordered = value as IOrderedQueryable;
            if (ordered != null)
                return new NullsafeOrderedQuery(ordered);

            return new NullsafeQuery(value);
        }

        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable ToNullsafe(this IOrderedQueryable value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new NullsafeOrderedQuery(value);
        }

        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable<T> ToNullsafe<T>(this IQueryable<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var ordered = value as IOrderedQueryable<T>;
            if (ordered != null)
                return new NullsafeOrderedQuery<T>(ordered);

            return new NullsafeQuery<T>(value);
        }

        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable<T> ToNullsafe<T>(this IOrderedQueryable<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new NullsafeOrderedQuery<T>(value);
        }
    }
}
