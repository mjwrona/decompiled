// Decompiled with JetBrains decompiler
// Type: Nest.SpanOrQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SpanOrQuery : QueryBase, ISpanOrQuery, ISpanSubQuery, IQuery
  {
    public IEnumerable<ISpanQuery> Clauses { get; set; }

    protected override bool Conditionless => SpanOrQuery.IsConditionless((ISpanOrQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanOr = (ISpanOrQuery) this;

    internal static bool IsConditionless(ISpanOrQuery q) => !q.Clauses.HasAny<ISpanQuery>() || q.Clauses.Cast<IQuery>().All<IQuery>((Func<IQuery, bool>) (qq => qq.Conditionless));
  }
}
