// Decompiled with JetBrains decompiler
// Type: Nest.SpanWithinQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanWithinQueryDescriptor<T> : 
    QueryDescriptorBase<SpanWithinQueryDescriptor<T>, ISpanWithinQuery>,
    ISpanWithinQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanWithinQuery.IsConditionless((ISpanWithinQuery) this);

    ISpanQuery ISpanWithinQuery.Big { get; set; }

    ISpanQuery ISpanWithinQuery.Little { get; set; }

    public SpanWithinQueryDescriptor<T> Little(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanWithinQuery, ISpanQuery>) ((a, v) => a.Little = v));

    public SpanWithinQueryDescriptor<T> Big(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanWithinQuery, ISpanQuery>) ((a, v) => a.Big = v));
  }
}
