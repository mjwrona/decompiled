// Decompiled with JetBrains decompiler
// Type: Nest.SpanContainingQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanContainingQueryDescriptor<T> : 
    QueryDescriptorBase<SpanContainingQueryDescriptor<T>, ISpanContainingQuery>,
    ISpanContainingQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanContainingQuery.IsConditionless((ISpanContainingQuery) this);

    ISpanQuery ISpanContainingQuery.Big { get; set; }

    ISpanQuery ISpanContainingQuery.Little { get; set; }

    public SpanContainingQueryDescriptor<T> Little(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanContainingQuery, ISpanQuery>) ((a, v) => a.Little = v));

    public SpanContainingQueryDescriptor<T> Big(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanContainingQuery, ISpanQuery>) ((a, v) => a.Big = v));
  }
}
