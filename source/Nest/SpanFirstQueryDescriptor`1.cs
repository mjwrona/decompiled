// Decompiled with JetBrains decompiler
// Type: Nest.SpanFirstQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanFirstQueryDescriptor<T> : 
    QueryDescriptorBase<SpanFirstQueryDescriptor<T>, ISpanFirstQuery>,
    ISpanFirstQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanFirstQuery.IsConditionless((ISpanFirstQuery) this);

    int? ISpanFirstQuery.End { get; set; }

    ISpanQuery ISpanFirstQuery.Match { get; set; }

    public SpanFirstQueryDescriptor<T> Match(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanFirstQuery, ISpanQuery>) ((a, v) => a.Match = v));

    public SpanFirstQueryDescriptor<T> End(int? end) => this.Assign<int?>(end, (Action<ISpanFirstQuery, int?>) ((a, v) => a.End = v));
  }
}
