// Decompiled with JetBrains decompiler
// Type: Nest.SpanQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanQueryDescriptor<T> : 
    QueryDescriptorBase<SpanQueryDescriptor<T>, ISpanQuery>,
    ISpanQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanQuery.IsConditionless((ISpanQuery) this);

    ISpanContainingQuery ISpanQuery.SpanContaining { get; set; }

    ISpanFieldMaskingQuery ISpanQuery.SpanFieldMasking { get; set; }

    ISpanFirstQuery ISpanQuery.SpanFirst { get; set; }

    ISpanGapQuery ISpanQuery.SpanGap { get; set; }

    ISpanMultiTermQuery ISpanQuery.SpanMultiTerm { get; set; }

    ISpanNearQuery ISpanQuery.SpanNear { get; set; }

    ISpanNotQuery ISpanQuery.SpanNot { get; set; }

    ISpanOrQuery ISpanQuery.SpanOr { get; set; }

    ISpanTermQuery ISpanQuery.SpanTerm { get; set; }

    ISpanWithinQuery ISpanQuery.SpanWithin { get; set; }

    void ISpanQuery.Accept(IQueryVisitor visitor) => new QueryWalker().Walk((ISpanQuery) this, visitor);

    public SpanQueryDescriptor<T> SpanTerm(
      Func<SpanTermQueryDescriptor<T>, ISpanTermQuery> selector)
    {
      return this.Assign<Func<SpanTermQueryDescriptor<T>, ISpanTermQuery>>(selector, (Action<ISpanQuery, Func<SpanTermQueryDescriptor<T>, ISpanTermQuery>>) ((a, v) => a.SpanTerm = v != null ? v(new SpanTermQueryDescriptor<T>()) : (ISpanTermQuery) null));
    }

    public SpanQueryDescriptor<T> SpanFirst(
      Func<SpanFirstQueryDescriptor<T>, ISpanFirstQuery> selector)
    {
      return this.Assign<Func<SpanFirstQueryDescriptor<T>, ISpanFirstQuery>>(selector, (Action<ISpanQuery, Func<SpanFirstQueryDescriptor<T>, ISpanFirstQuery>>) ((a, v) => a.SpanFirst = v != null ? v(new SpanFirstQueryDescriptor<T>()) : (ISpanFirstQuery) null));
    }

    public SpanQueryDescriptor<T> SpanNear(
      Func<SpanNearQueryDescriptor<T>, ISpanNearQuery> selector)
    {
      return this.Assign<Func<SpanNearQueryDescriptor<T>, ISpanNearQuery>>(selector, (Action<ISpanQuery, Func<SpanNearQueryDescriptor<T>, ISpanNearQuery>>) ((a, v) => a.SpanNear = v != null ? v(new SpanNearQueryDescriptor<T>()) : (ISpanNearQuery) null));
    }

    public SpanQueryDescriptor<T> SpanGap(
      Func<SpanGapQueryDescriptor<T>, ISpanGapQuery> selector)
    {
      return this.Assign<Func<SpanGapQueryDescriptor<T>, ISpanGapQuery>>(selector, (Action<ISpanQuery, Func<SpanGapQueryDescriptor<T>, ISpanGapQuery>>) ((a, v) => a.SpanGap = v != null ? v(new SpanGapQueryDescriptor<T>()) : (ISpanGapQuery) null));
    }

    public SpanQueryDescriptor<T> SpanOr(
      Func<SpanOrQueryDescriptor<T>, ISpanOrQuery> selector)
    {
      return this.Assign<Func<SpanOrQueryDescriptor<T>, ISpanOrQuery>>(selector, (Action<ISpanQuery, Func<SpanOrQueryDescriptor<T>, ISpanOrQuery>>) ((a, v) => a.SpanOr = v != null ? v(new SpanOrQueryDescriptor<T>()) : (ISpanOrQuery) null));
    }

    public SpanQueryDescriptor<T> SpanNot(
      Func<SpanNotQueryDescriptor<T>, ISpanNotQuery> selector)
    {
      return this.Assign<Func<SpanNotQueryDescriptor<T>, ISpanNotQuery>>(selector, (Action<ISpanQuery, Func<SpanNotQueryDescriptor<T>, ISpanNotQuery>>) ((a, v) => a.SpanNot = v != null ? v(new SpanNotQueryDescriptor<T>()) : (ISpanNotQuery) null));
    }

    public SpanQueryDescriptor<T> SpanMultiTerm(
      Func<SpanMultiTermQueryDescriptor<T>, ISpanMultiTermQuery> selector)
    {
      return this.Assign<Func<SpanMultiTermQueryDescriptor<T>, ISpanMultiTermQuery>>(selector, (Action<ISpanQuery, Func<SpanMultiTermQueryDescriptor<T>, ISpanMultiTermQuery>>) ((a, v) => a.SpanMultiTerm = v != null ? v(new SpanMultiTermQueryDescriptor<T>()) : (ISpanMultiTermQuery) null));
    }

    public SpanQueryDescriptor<T> SpanContaining(
      Func<SpanContainingQueryDescriptor<T>, ISpanContainingQuery> selector)
    {
      return this.Assign<Func<SpanContainingQueryDescriptor<T>, ISpanContainingQuery>>(selector, (Action<ISpanQuery, Func<SpanContainingQueryDescriptor<T>, ISpanContainingQuery>>) ((a, v) => a.SpanContaining = v != null ? v(new SpanContainingQueryDescriptor<T>()) : (ISpanContainingQuery) null));
    }

    public SpanQueryDescriptor<T> SpanWithin(
      Func<SpanWithinQueryDescriptor<T>, ISpanWithinQuery> selector)
    {
      return this.Assign<Func<SpanWithinQueryDescriptor<T>, ISpanWithinQuery>>(selector, (Action<ISpanQuery, Func<SpanWithinQueryDescriptor<T>, ISpanWithinQuery>>) ((a, v) => a.SpanWithin = v != null ? v(new SpanWithinQueryDescriptor<T>()) : (ISpanWithinQuery) null));
    }

    public SpanQueryDescriptor<T> SpanFieldMasking(
      Func<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery> selector)
    {
      return this.Assign<Func<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery>>(selector, (Action<ISpanQuery, Func<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery>>) ((a, v) => a.SpanFieldMasking = v != null ? v(new SpanFieldMaskingQueryDescriptor<T>()) : (ISpanFieldMaskingQuery) null));
    }
  }
}
