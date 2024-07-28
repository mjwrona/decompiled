// Decompiled with JetBrains decompiler
// Type: Nest.BoostingQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class BoostingQuery : QueryBase, IBoostingQuery, IQuery
  {
    public double? NegativeBoost { get; set; }

    public QueryContainer NegativeQuery { get; set; }

    public QueryContainer PositiveQuery { get; set; }

    protected override bool Conditionless => BoostingQuery.IsConditionless((IBoostingQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Boosting = (IBoostingQuery) this;

    internal static bool IsConditionless(IBoostingQuery q) => q.NegativeQuery.NotWritable() && q.PositiveQuery.NotWritable();
  }
}
