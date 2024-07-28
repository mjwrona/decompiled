// Decompiled with JetBrains decompiler
// Type: Nest.SpanOrQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SpanOrQueryDescriptor<T> : 
    QueryDescriptorBase<SpanOrQueryDescriptor<T>, ISpanOrQuery>,
    ISpanOrQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanOrQuery.IsConditionless((ISpanOrQuery) this);

    IEnumerable<ISpanQuery> ISpanOrQuery.Clauses { get; set; }

    public SpanOrQueryDescriptor<T> Clauses(
      params Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>[] selectors)
    {
      return this.Assign<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>[]>(selectors, (Action<ISpanOrQuery, Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>[]>) ((a, v) =>
      {
        List<SpanQueryDescriptor<T>> list = ((IEnumerable<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>>) v).Select<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>, SpanQueryDescriptor<T>>((Func<Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>>, SpanQueryDescriptor<T>>) (selector => selector(new SpanQueryDescriptor<T>()))).Where<SpanQueryDescriptor<T>>((Func<SpanQueryDescriptor<T>, bool>) (q => !((IQuery) q).Conditionless)).ToList<SpanQueryDescriptor<T>>();
        a.Clauses = list.HasAny<SpanQueryDescriptor<T>>() ? (IEnumerable<ISpanQuery>) list : (IEnumerable<ISpanQuery>) null;
      }));
    }
  }
}
