// Decompiled with JetBrains decompiler
// Type: Nest.HasParentQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class HasParentQuery : QueryBase, IHasParentQuery, IQuery
  {
    public bool? IgnoreUnmapped { get; set; }

    public IInnerHits InnerHits { get; set; }

    public RelationName ParentType { get; set; }

    public QueryContainer Query { get; set; }

    public bool? Score { get; set; }

    protected override bool Conditionless => HasParentQuery.IsConditionless((IHasParentQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.HasParent = (IHasParentQuery) this;

    internal static bool IsConditionless(IHasParentQuery q) => q.Query == null || q.Query.IsConditionless || q.ParentType == (RelationName) null;
  }
}
