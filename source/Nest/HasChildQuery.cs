// Decompiled with JetBrains decompiler
// Type: Nest.HasChildQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class HasChildQuery : QueryBase, IHasChildQuery, IQuery
  {
    public bool? IgnoreUnmapped { get; set; }

    public IInnerHits InnerHits { get; set; }

    public int? MaxChildren { get; set; }

    public int? MinChildren { get; set; }

    public QueryContainer Query { get; set; }

    public ChildScoreMode? ScoreMode { get; set; }

    public RelationName Type { get; set; }

    protected override bool Conditionless => HasChildQuery.IsConditionless((IHasChildQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.HasChild = (IHasChildQuery) this;

    internal static bool IsConditionless(IHasChildQuery q) => q.Query == null || q.Query.IsConditionless || q.Type == (RelationName) null;
  }
}
