// Decompiled with JetBrains decompiler
// Type: Nest.BoostingQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class BoostingQueryDescriptor<T> : 
    QueryDescriptorBase<BoostingQueryDescriptor<T>, IBoostingQuery>,
    IBoostingQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => BoostingQuery.IsConditionless((IBoostingQuery) this);

    double? IBoostingQuery.NegativeBoost { get; set; }

    QueryContainer IBoostingQuery.NegativeQuery { get; set; }

    QueryContainer IBoostingQuery.PositiveQuery { get; set; }

    public BoostingQueryDescriptor<T> NegativeBoost(double? boost) => this.Assign<double?>(boost, (Action<IBoostingQuery, double?>) ((a, v) => a.NegativeBoost = v));

    public BoostingQueryDescriptor<T> Positive(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IBoostingQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.PositiveQuery = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public BoostingQueryDescriptor<T> Negative(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IBoostingQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.NegativeQuery = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
