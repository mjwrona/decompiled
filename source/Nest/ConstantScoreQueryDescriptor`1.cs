// Decompiled with JetBrains decompiler
// Type: Nest.ConstantScoreQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ConstantScoreQueryDescriptor<T> : 
    QueryDescriptorBase<ConstantScoreQueryDescriptor<T>, IConstantScoreQuery>,
    IConstantScoreQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => ConstantScoreQuery.IsConditionless((IConstantScoreQuery) this);

    QueryContainer IConstantScoreQuery.Filter { get; set; }

    public ConstantScoreQueryDescriptor<T> Filter(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IConstantScoreQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
