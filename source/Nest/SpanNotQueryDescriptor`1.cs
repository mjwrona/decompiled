// Decompiled with JetBrains decompiler
// Type: Nest.SpanNotQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanNotQueryDescriptor<T> : 
    QueryDescriptorBase<SpanNotQueryDescriptor<T>, ISpanNotQuery>,
    ISpanNotQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanNotQuery.IsConditionless((ISpanNotQuery) this);

    int? ISpanNotQuery.Dist { get; set; }

    ISpanQuery ISpanNotQuery.Exclude { get; set; }

    ISpanQuery ISpanNotQuery.Include { get; set; }

    int? ISpanNotQuery.Post { get; set; }

    int? ISpanNotQuery.Pre { get; set; }

    public SpanNotQueryDescriptor<T> Include(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanNotQuery, ISpanQuery>) ((a, v) => a.Include = v));

    public SpanNotQueryDescriptor<T> Exclude(Func<SpanQueryDescriptor<T>, ISpanQuery> selector) => this.Assign<ISpanQuery>(selector(new SpanQueryDescriptor<T>()), (Action<ISpanNotQuery, ISpanQuery>) ((a, v) => a.Exclude = v));

    public SpanNotQueryDescriptor<T> Pre(int? pre) => this.Assign<int?>(pre, (Action<ISpanNotQuery, int?>) ((a, v) => a.Pre = v));

    public SpanNotQueryDescriptor<T> Post(int? post) => this.Assign<int?>(post, (Action<ISpanNotQuery, int?>) ((a, v) => a.Post = v));

    public SpanNotQueryDescriptor<T> Dist(int? dist) => this.Assign<int?>(dist, (Action<ISpanNotQuery, int?>) ((a, v) => a.Dist = v));
  }
}
