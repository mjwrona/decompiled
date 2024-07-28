// Decompiled with JetBrains decompiler
// Type: Nest.RescoreQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RescoreQueryDescriptor<T> : 
    DescriptorBase<RescoreQueryDescriptor<T>, IRescoreQuery>,
    IRescoreQuery
    where T : class
  {
    QueryContainer IRescoreQuery.Query { get; set; }

    double? IRescoreQuery.QueryWeight { get; set; }

    double? IRescoreQuery.RescoreQueryWeight { get; set; }

    Nest.ScoreMode? IRescoreQuery.ScoreMode { get; set; }

    public virtual RescoreQueryDescriptor<T> QueryWeight(double? queryWeight) => this.Assign<double?>(queryWeight, (Action<IRescoreQuery, double?>) ((a, v) => a.QueryWeight = v));

    public virtual RescoreQueryDescriptor<T> RescoreQueryWeight(double? rescoreQueryWeight) => this.Assign<double?>(rescoreQueryWeight, (Action<IRescoreQuery, double?>) ((a, v) => a.RescoreQueryWeight = v));

    public virtual RescoreQueryDescriptor<T> ScoreMode(Nest.ScoreMode? scoreMode) => this.Assign<Nest.ScoreMode?>(scoreMode, (Action<IRescoreQuery, Nest.ScoreMode?>) ((a, v) => a.ScoreMode = v));

    public virtual RescoreQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(query, (Action<IRescoreQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
