// Decompiled with JetBrains decompiler
// Type: Nest.ConditionlessQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ConditionlessQueryDescriptor<T> : 
    QueryDescriptorBase<ConditionlessQueryDescriptor<T>, IConditionlessQuery>,
    IConditionlessQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless
    {
      get
      {
        if (this.Self.Query != null && !this.Self.Query.IsConditionless)
          return false;
        return this.Self.Fallback == null || this.Self.Fallback.IsConditionless;
      }
    }

    QueryContainer IConditionlessQuery.Fallback { get; set; }

    QueryContainer IConditionlessQuery.Query { get; set; }

    public ConditionlessQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IConditionlessQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public ConditionlessQueryDescriptor<T> Fallback(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IConditionlessQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Fallback = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
