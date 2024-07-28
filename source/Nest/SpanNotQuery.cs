// Decompiled with JetBrains decompiler
// Type: Nest.SpanNotQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SpanNotQuery : QueryBase, ISpanNotQuery, ISpanSubQuery, IQuery
  {
    public int? Dist { get; set; }

    public ISpanQuery Exclude { get; set; }

    public ISpanQuery Include { get; set; }

    public int? Post { get; set; }

    public int? Pre { get; set; }

    protected override bool Conditionless => SpanNotQuery.IsConditionless((ISpanNotQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanNot = (ISpanNotQuery) this;

    internal static bool IsConditionless(ISpanNotQuery q)
    {
      IQuery exclude = (IQuery) q.Exclude;
      IQuery include = (IQuery) q.Include;
      if (exclude == null && include == null || include == null && exclude.Conditionless || exclude == null && include.Conditionless)
        return true;
      return exclude != null && exclude.Conditionless && include != null && include.Conditionless;
    }
  }
}
