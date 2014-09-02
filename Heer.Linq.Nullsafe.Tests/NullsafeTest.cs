using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Heer.Linq.Nullsafe.Tests
{
    public class NullsafeTest
    {
        private IQueryable<B> query;

        public NullsafeTest()
        {
            var data = new[] {
                new A
                {
                    SomeInteger = 7,
                    SomeDate = new DateTime(1977, 05, 25),
                    SomeOther = new A { SomeInteger = 42 }
                },
                new A
                {
                    SomeInteger = 1138,
                    SomeDate = new DateTime(1980, 05, 21),
                    SomeOthers = new[]
                    {
                        new A { SomeDate = new DateTime(2000, 3, 1) },
                        new A { SomeDate = new DateTime(2000, 6, 1) }
                    }
                },
                new A
                {
                    SomeInteger = 123456,
                    SomeDate = new DateTime(1983, 05, 25),
                    MoreOthers = new[]
                    {
                        new A { SomeOther = new A { SomeDate = new DateTime(2000, 1, 5) } },
                        new A { SomeOther = new A { SomeDate = new DateTime(2000, 1, 8) } }
                    }
                },
                null
            };

            query = from a in data.AsQueryable()
                    orderby a.SomeInteger
                    select new B
                    {
                        Year = a.SomeDate.Year,
                        Integer = a.SomeOther.SomeInteger,
                        Others = from b in a.SomeOthers
                                 select b.SomeDate.Month,
                        More = from c in a.MoreOthers
                               select c.SomeOther.SomeDate.Day
                    };
        }

        [Fact]
        public void OrdinaryQueryShouldFail()
        {
            Assert.Throws<NullReferenceException>(() =>
                query.ToList());
        }

        [Fact]
        public void NullsafeQueryShouldSucceed()
        {
            var result = query.ToNullsafe().ToList();

            Assert.Equal(4, result.Count);

            Assert.Equal(0, result[0].Year);
            Assert.Equal(0, result[0].Integer);
            Assert.Equal(new int[0], result[0].Others.ToList());
            Assert.Equal(new int[0], result[0].More.ToList());

            Assert.Equal(1977, result[1].Year);
            Assert.Equal(42, result[1].Integer);
            Assert.Equal(new int[0], result[1].Others.ToList());
            Assert.Equal(new int[0], result[1].More.ToList());

            Assert.Equal(1980, result[2].Year);
            Assert.Equal(0, result[2].Integer);
            Assert.Equal(new[] { 3, 6 }, result[2].Others.ToList());
            Assert.Equal(new int[0], result[2].More.ToList());

            Assert.Equal(1983, result[3].Year);
            Assert.Equal(0, result[3].Integer);
            Assert.Equal(new int[0], result[3].Others.ToList());
            Assert.Equal(new[] { 5, 8 }, result[3].More.ToList());
        }

        private class A
        {
            public int SomeInteger { get; set; }

            public DateTime SomeDate { get; set; }

            public A SomeOther { get; set; }

            public IEnumerable<A> SomeOthers { get; set; }

            public ICollection<A> MoreOthers { get; set; }
        }

        private class B
        {
            public int Year { get; set; }

            public int Integer { get; set; }

            public IEnumerable<int> Others { get; set; }

            public IEnumerable<int> More { get; set; }
        }
    }
}