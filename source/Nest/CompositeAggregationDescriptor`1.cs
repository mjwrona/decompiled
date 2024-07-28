// Decompiled with JetBrains decompiler
// Type: Nest.CompositeAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CompositeAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<CompositeAggregationDescriptor<T>, ICompositeAggregation, T>,
    ICompositeAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    CompositeKey ICompositeAggregation.After { get; set; }

    int? ICompositeAggregation.Size { get; set; }

    IEnumerable<ICompositeAggregationSource> ICompositeAggregation.Sources { get; set; }

    public CompositeAggregationDescriptor<T> Sources(
      Func<CompositeAggregationSourcesDescriptor<T>, IPromise<IList<ICompositeAggregationSource>>> selector)
    {
      return this.Assign<Func<CompositeAggregationSourcesDescriptor<T>, IPromise<IList<ICompositeAggregationSource>>>>(selector, (Action<ICompositeAggregation, Func<CompositeAggregationSourcesDescriptor<T>, IPromise<IList<ICompositeAggregationSource>>>>) ((a, v) => a.Sources = v != null ? (IEnumerable<ICompositeAggregationSource>) v(new CompositeAggregationSourcesDescriptor<T>())?.Value : (IEnumerable<ICompositeAggregationSource>) null));
    }

    public CompositeAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<ICompositeAggregation, int?>) ((a, v) => a.Size = v));

    public CompositeAggregationDescriptor<T> After(CompositeKey after) => this.Assign<CompositeKey>(after, (Action<ICompositeAggregation, CompositeKey>) ((a, v) => a.After = v));
  }
}
