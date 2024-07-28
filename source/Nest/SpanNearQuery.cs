// Decompiled with JetBrains decompiler
// Type: Nest.SpanNearQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SpanNearQuery : QueryBase, ISpanNearQuery, ISpanSubQuery, IQuery
  {
    public IEnumerable<ISpanQuery> Clauses { get; set; }

    public bool? InOrder { get; set; }

    public int? Slop { get; set; }

    protected override bool Conditionless => SpanNearQuery.IsConditionless((ISpanNearQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanNear = (ISpanNearQuery) this;

    internal static bool IsConditionless(ISpanNearQuery q) => !q.Clauses.HasAny<ISpanQuery>() || q.Clauses.Cast<IQuery>().All<IQuery>((Func<IQuery, bool>) (qq => qq.Conditionless));
  }
}
