// Decompiled with JetBrains decompiler
// Type: Nest.ExistsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ExistsQuery : QueryBase, IExistsQuery, IQuery
  {
    public Field Field { get; set; }

    protected override bool Conditionless => ExistsQuery.IsConditionless((IExistsQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Exists = (IExistsQuery) this;

    internal static bool IsConditionless(IExistsQuery q) => q.Field.IsConditionless();
  }
}
