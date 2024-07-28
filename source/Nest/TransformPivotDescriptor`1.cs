// Decompiled with JetBrains decompiler
// Type: Nest.TransformPivotDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TransformPivotDescriptor<TDocument> : 
    DescriptorBase<TransformPivotDescriptor<TDocument>, ITransformPivot>,
    ITransformPivot
    where TDocument : class
  {
    int? ITransformPivot.MaxPageSearchSize { get; set; }

    AggregationDictionary ITransformPivot.Aggregations { get; set; }

    IDictionary<string, ISingleGroupSource> ITransformPivot.GroupBy { get; set; }

    public TransformPivotDescriptor<TDocument> MaxPageSearchSize(int? maxPageSearchSize) => this.Assign<int?>(maxPageSearchSize, (Action<ITransformPivot, int?>) ((a, v) => a.MaxPageSearchSize = v));

    public TransformPivotDescriptor<TDocument> Aggregations(
      Func<AggregationContainerDescriptor<TDocument>, IAggregationContainer> aggregationsSelector)
    {
      return this.Assign<AggregationDictionary>(aggregationsSelector(new AggregationContainerDescriptor<TDocument>())?.Aggregations, (Action<ITransformPivot, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public TransformPivotDescriptor<TDocument> Aggregations(AggregationDictionary aggregations) => this.Assign<AggregationDictionary>(aggregations, (Action<ITransformPivot, AggregationDictionary>) ((a, v) => a.Aggregations = v));

    public TransformPivotDescriptor<TDocument> GroupBy(
      Func<SingleGroupSourcesDescriptor<TDocument>, IPromise<IDictionary<string, ISingleGroupSource>>> selector)
    {
      return this.Assign<Func<SingleGroupSourcesDescriptor<TDocument>, IPromise<IDictionary<string, ISingleGroupSource>>>>(selector, (Action<ITransformPivot, Func<SingleGroupSourcesDescriptor<TDocument>, IPromise<IDictionary<string, ISingleGroupSource>>>>) ((a, v) => a.GroupBy = v != null ? v(new SingleGroupSourcesDescriptor<TDocument>())?.Value : (IDictionary<string, ISingleGroupSource>) null));
    }
  }
}
