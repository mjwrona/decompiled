// Decompiled with JetBrains decompiler
// Type: Nest.SpanQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SpanQuery : ISpanQuery, IQuery
  {
    public bool IsStrict { get; set; }

    public bool IsVerbatim { get; set; }

    public bool IsWritable => this.IsVerbatim || !SpanQuery.IsConditionless((ISpanQuery) this);

    public ISpanContainingQuery SpanContaining { get; set; }

    public ISpanFieldMaskingQuery SpanFieldMasking { get; set; }

    public ISpanFirstQuery SpanFirst { get; set; }

    public ISpanGapQuery SpanGap { get; set; }

    public ISpanMultiTermQuery SpanMultiTerm { get; set; }

    public ISpanNearQuery SpanNear { get; set; }

    public ISpanNotQuery SpanNot { get; set; }

    public ISpanOrQuery SpanOr { get; set; }

    public ISpanTermQuery SpanTerm { get; set; }

    public ISpanWithinQuery SpanWithin { get; set; }

    double? IQuery.Boost { get; set; }

    bool IQuery.Conditionless => SpanQuery.IsConditionless((ISpanQuery) this);

    bool IQuery.IsWritable => this.IsWritable;

    string IQuery.Name { get; set; }

    public void Accept(IQueryVisitor visitor) => new QueryWalker().Walk((ISpanQuery) this, visitor);

    internal static bool IsConditionless(ISpanQuery q) => ((IEnumerable<IQuery>) new IQuery[8]
    {
      (IQuery) q.SpanTerm,
      (IQuery) q.SpanFirst,
      (IQuery) q.SpanNear,
      (IQuery) q.SpanOr,
      (IQuery) q.SpanNot,
      (IQuery) q.SpanMultiTerm,
      (IQuery) q.SpanGap,
      (IQuery) q.SpanFieldMasking
    }).All<IQuery>((Func<IQuery, bool>) (sq => sq == null || sq.Conditionless));
  }
}
