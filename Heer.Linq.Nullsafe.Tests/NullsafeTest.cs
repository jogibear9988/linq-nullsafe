using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Heer.Linq.Nullsafe.Tests
{
    [TestClass]
    public class NullsafeTest
    {
        private IQueryable<B> query;

        [TestInitialize]
        public void Initialize()
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

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void OrdinaryQueryShouldFail()
        {
            var result = query.ToList();
        }

        [TestMethod]
        public void NullsafeQueryShouldSucceed()
        {
            var result = query.ToNullsafe().ToList();

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(0, result[0].Year);
            Assert.AreEqual(0, result[0].Integer);
            CollectionAssert.AreEquivalent(new int[0], result[0].Others.ToList());
            CollectionAssert.AreEquivalent(new int[0], result[0].More.ToList());

            Assert.AreEqual(1977, result[1].Year);
            Assert.AreEqual(42, result[1].Integer);
            CollectionAssert.AreEquivalent(new int[0], result[1].Others.ToList());
            CollectionAssert.AreEquivalent(new int[0], result[1].More.ToList());

            Assert.AreEqual(1980, result[2].Year);
            Assert.AreEqual(0, result[2].Integer);
            CollectionAssert.AreEquivalent(new[] { 3, 6 }, result[2].Others.ToList());
            CollectionAssert.AreEquivalent(new int[0], result[2].More.ToList());

            Assert.AreEqual(1983, result[3].Year);
            Assert.AreEqual(0, result[3].Integer);
            CollectionAssert.AreEquivalent(new int[0], result[3].Others.ToList());
            CollectionAssert.AreEquivalent(new[] { 5, 8 }, result[3].More.ToList());
        }

        class A
        {
            public int SomeInteger { get; set; }
            public DateTime SomeDate { get; set; }
            public A SomeOther { get; set; }
            public IEnumerable<A> SomeOthers { get; set; }
            public ICollection<A> MoreOthers { get; set; }
        }

        class B
        {
            public int Year { get; set; }
            public int Integer { get; set; }
            public IEnumerable<int> Others { get; set; }
            public IEnumerable<int> More { get; set; }
        }
    }
}
