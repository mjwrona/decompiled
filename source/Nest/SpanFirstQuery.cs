// Decompiled with JetBrains decompiler
// Type: Nest.SpanFirstQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SpanFirstQuery : QueryBase, ISpanFirstQuery, ISpanSubQuery, IQuery
  {
    public int? End { get; set; }

    public ISpanQuery Match { get; set; }

    protected override bool Conditionless => SpanFirstQuery.IsConditionless((ISpanFirstQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanFirst = (ISpanFirstQuery) this;

    internal static bool IsConditionless(ISpanFirstQuery q) => q.Match == null || q.Match.Conditionless;
  }
}
