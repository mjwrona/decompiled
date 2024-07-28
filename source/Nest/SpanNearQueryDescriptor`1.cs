// Decompiled with JetBrains decompiler
// Type: Nest.SpanNearQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SpanNearQueryDescriptor<T> : 
    QueryDescriptorBase<SpanNearQueryDescriptor<T>, ISpanNearQuery>,
    ISpanNearQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanNearQuery.IsConditionless((ISpanNearQuery) this);

    IEnumerable<ISpanQuery> ISpanNearQuery.Clauses { get; set; }

    bool? ISpanNearQuery.InOrder { get; set; }

    int? ISpanNearQuery.Slop { get; set; }

    public SpanNearQueryDescriptor<T> Clauses(
      params Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>[] selectors)
    {
      return this.Clauses((IEnumerable<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>>) ((IEnumerable<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>>) selectors).ToList<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>>());
    }

    public SpanNearQueryDescriptor<T> Clauses(
      IEnumerable<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>> selectors)
    {
      return this.Assign<IEnumerable<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>>>(selectors, (Action<ISpanNearQuery, IEnumerable<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>>>) ((a, v) => a.Clauses = (IEnumerable<ISpanQuery>) v.Select<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>, SpanQueryDescriptor<T>>((Func<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>, SpanQueryDescriptor<T>>) (selector => selector == null ? (SpanQueryDescriptor<T>) null : selector(new SpanQueryDescriptor<T>()))).Where<SpanQueryDescriptor<T>>((Func<SpanQueryDescriptor<T>, bool>) (query => query != null && !((IQuery) query).Conditionless)).ToListOrNullIfEmpty<SpanQueryDescriptor<T>>()));
    }

    public SpanNearQueryDescriptor<T> Slop(int? slop) => this.Assign<int?>(slop, (Action<ISpanNearQuery, int?>) ((a, v) => a.Slop = v));

    public SpanNearQueryDescriptor<T> InOrder(bool? inOrder = true) => this.Assign<bool?>(inOrder, (Action<ISpanNearQuery, bool?>) ((a, v) => a.InOrder = v));
  }
}
