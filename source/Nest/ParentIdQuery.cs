// Decompiled with JetBrains decompiler
// Type: Nest.ParentIdQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ParentIdQuery : QueryBase, IParentIdQuery, IQuery
  {
    public Id Id { get; set; }

    public bool? IgnoreUnmapped { get; set; }

    public RelationName Type { get; set; }

    protected override bool Conditionless => ParentIdQuery.IsConditionless((IParentIdQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.ParentId = (IParentIdQuery) this;

    internal static bool IsConditionless(IParentIdQuery q) => q.Type.IsConditionless() || q.Id.IsConditionless();
  }
}
