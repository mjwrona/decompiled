// Decompiled with JetBrains decompiler
// Type: Nest.DisMaxQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DisMaxQueryDescriptor<T> : 
    QueryDescriptorBase<DisMaxQueryDescriptor<T>, IDisMaxQuery>,
    IDisMaxQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => DisMaxQuery.IsConditionless((IDisMaxQuery) this);

    IEnumerable<QueryContainer> IDisMaxQuery.Queries { get; set; }

    double? IDisMaxQuery.TieBreaker { get; set; }

    public DisMaxQueryDescriptor<T> Queries(
      params Func<QueryContainerDescriptor<T>, QueryContainer>[] querySelectors)
    {
      return this.Assign<List<QueryContainer>>(((IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>) querySelectors).Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).Where<QueryContainer>((Func<QueryContainer, bool>) (q => q != null)).ToListOrNullIfEmpty<QueryContainer>(), (Action<IDisMaxQuery, List<QueryContainer>>) ((a, v) => a.Queries = (IEnumerable<QueryContainer>) v));
    }

    public DisMaxQueryDescriptor<T> Queries(
      IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> querySelectors)
    {
      return this.Assign<List<QueryContainer>>(querySelectors.Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).Where<QueryContainer>((Func<QueryContainer, bool>) (q => q != null)).ToListOrNullIfEmpty<QueryContainer>(), (Action<IDisMaxQuery, List<QueryContainer>>) ((a, v) => a.Queries = (IEnumerable<QueryContainer>) v));
    }

    public DisMaxQueryDescriptor<T> Queries(params QueryContainer[] queries) => this.Assign<List<QueryContainer>>(((IEnumerable<QueryContainer>) queries).Where<QueryContainer>((Func<QueryContainer, bool>) (q => q != null)).ToListOrNullIfEmpty<QueryContainer>(), (Action<IDisMaxQuery, List<QueryContainer>>) ((a, v) => a.Queries = (IEnumerable<QueryContainer>) v));

    public DisMaxQueryDescriptor<T> TieBreaker(double? tieBreaker) => this.Assign<double?>(tieBreaker, (Action<IDisMaxQuery, double?>) ((a, v) => a.TieBreaker = v));
  }
}
