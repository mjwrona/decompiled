// Decompiled with JetBrains decompiler
// Type: Nest.PinnedQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class PinnedQuery : QueryBase, IPinnedQuery, IQuery
  {
    public IEnumerable<Id> Ids { get; set; }

    public QueryContainer Organic { get; set; }

    protected override bool Conditionless => PinnedQuery.IsConditionless((IPinnedQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Pinned = (IPinnedQuery) this;

    internal static bool IsConditionless(IPinnedQuery q) => !q.Ids.HasAny<Id>() && q.Organic.IsConditionless();
  }
}
