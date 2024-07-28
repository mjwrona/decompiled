// Decompiled with JetBrains decompiler
// Type: Nest.QueryBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class QueryBase : IQuery
  {
    public double? Boost { get; set; }

    public bool IsStrict { get; set; }

    public bool IsVerbatim { get; set; }

    public bool IsWritable => this.IsVerbatim || !this.Conditionless;

    public string Name { get; set; }

    protected abstract bool Conditionless { get; }

    bool IQuery.Conditionless => this.Conditionless;

    public static bool operator false(QueryBase a) => false;

    public static bool operator true(QueryBase a) => false;

    public static QueryBase operator &(QueryBase leftQuery, QueryBase rightQuery) => QueryBase.Combine(leftQuery, rightQuery, (Func<QueryContainer, QueryContainer, QueryContainer>) ((l, r) =>
    {
      QueryContainer queryContainer = l;
      return queryContainer ? queryContainer & r : queryContainer;
    }));

    public static QueryBase operator |(QueryBase leftQuery, QueryBase rightQuery) => QueryBase.Combine(leftQuery, rightQuery, (Func<QueryContainer, QueryContainer, QueryContainer>) ((l, r) =>
    {
      QueryContainer queryContainer = l;
      return !queryContainer ? queryContainer | r : queryContainer;
    }));

    public static QueryBase operator !(QueryBase query)
    {
      if (query == null || !query.IsWritable)
        return (QueryBase) null;
      return (QueryBase) new BoolQuery()
      {
        MustNot = (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          (QueryContainer) query
        }
      };
    }

    public static QueryBase operator +(QueryBase query)
    {
      if (query == null || !query.IsWritable)
        return (QueryBase) null;
      return (QueryBase) new BoolQuery()
      {
        Filter = (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          (QueryContainer) query
        }
      };
    }

    private static QueryBase Combine(
      QueryBase leftQuery,
      QueryBase rightQuery,
      Func<QueryContainer, QueryContainer, QueryContainer> combine)
    {
      QueryBase query;
      if (QueryBase.IfEitherIsEmptyReturnTheOtherOrEmpty(leftQuery, rightQuery, out query))
        return query;
      IBoolQuery boolQuery = ((IQueryContainer) combine((QueryContainer) leftQuery, (QueryContainer) rightQuery)).Bool;
      return (QueryBase) new BoolQuery()
      {
        Must = boolQuery.Must,
        MustNot = boolQuery.MustNot,
        Should = boolQuery.Should,
        Filter = boolQuery.Filter
      };
    }

    private static bool IfEitherIsEmptyReturnTheOtherOrEmpty(
      QueryBase leftQuery,
      QueryBase rightQuery,
      out QueryBase query)
    {
      query = (QueryBase) null;
      if (leftQuery == null && rightQuery == null)
        return true;
      // ISSUE: explicit non-virtual call
      bool flag1 = leftQuery != null && __nonvirtual (leftQuery.IsWritable);
      // ISSUE: explicit non-virtual call
      bool flag2 = rightQuery != null && __nonvirtual (rightQuery.IsWritable);
      if (flag1 & flag2)
        return false;
      if (!flag1 && !flag2)
        return true;
      query = flag1 ? leftQuery : rightQuery;
      return true;
    }

    public static implicit operator QueryContainer(QueryBase query) => query != null ? new QueryContainer(query) : (QueryContainer) null;

    internal void WrapInContainer(IQueryContainer container)
    {
      container.IsVerbatim = this.IsVerbatim;
      container.IsStrict = this.IsStrict;
      this.InternalWrapInContainer(container);
    }

    internal abstract void InternalWrapInContainer(IQueryContainer container);
  }
}
