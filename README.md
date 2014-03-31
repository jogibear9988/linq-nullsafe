LINQ Nullsafe
=============

Ever tried to write unit tests or the like for complex LINQ queries, which are executed as SQL statements under ordinary conditions, but should be executed in memory under "test" conditions without triggering `NullReferenceException`s, one after the other? Or just being tired of these numerous null checks in your query expressions and waiting for the monadic null-check operator in C# 6.0?

This code provides some kind of "query provider proxy" injecting null checks in your query expressions. Automatically.

```csharp
// may trigger null references
query = from a in data
        orderby a.SomeInteger
        select new
        {
            Year    = a.SomeDate.Year,
            Integer = a.SomeOther.SomeInteger,
            Others  = from b in a.SomeOthers
                      select b.SomeDate.Month,
            More    = from c in a.MoreOthers
                      select c.SomeOther.SomeDate.Day
        };

// avoid null references the hard way
query = from a in data
        orderby a.SomeInteger
        select new
        {
            Year    = a.SomeDate.Year,
            Integer = a.SomeOther != null
                    ? a.SomeOther.SomeInteger
                    : 0,
            Others  = a.SomeOthers != null
                    ? from b in a.SomeOthers
                      select b.SomeDate.Month
                    : null,
            More    = a.MoreOthers != null
                    ? from c in a.MoreOthers
                      select c.SomeOther != null
                           ? c.SomeOther.SomeDate.Day
                           : 0
                    : null
        };

// avoid null references with more style
query = from a in data.ToNullsafe()
        orderby a.SomeInteger
        select new
        {
            Year    = a.SomeDate.Year,
            Integer = a.SomeOther.SomeInteger,
            Others  = from b in a.SomeOthers
                      select b.SomeDate.Month,
            More    = from c in a.MoreOthers
                      select c.SomeOther.SomeDate.Day
        };
```

The query above contains many member access expressions -- these are quite safe in some query providers, but are definitely dangerous in ordinary enumerable queries. Boom!

Just get the code and try it yourself. :)
