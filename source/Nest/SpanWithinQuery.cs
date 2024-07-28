// Decompiled with JetBrains decompiler
// Type: Nest.SpanWithinQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SpanWithinQuery : QueryBase, ISpanWithinQuery, ISpanSubQuery, IQuery
  {
    public ISpanQuery Big { get; set; }

    public ISpanQuery Little { get; set; }

    protected override bool Conditionless => SpanWithinQuery.IsConditionless((ISpanWithinQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanWithin = (ISpanWithinQuery) this;

    internal static bool IsConditionless(ISpanWithinQuery q)
    {
      IQuery little = (IQuery) q.Little;
      IQuery big = (IQuery) q.Big;
      if (little == null && big == null || big == null && little.Conditionless || little == null && big.Conditionless)
        return true;
      return little != null && little.Conditionless && big != null && big.Conditionless;
    }
  }
}
