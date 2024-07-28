// Decompiled with JetBrains decompiler
// Type: Nest.CompositeAggregationSourcesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CompositeAggregationSourcesDescriptor<T> : 
    DescriptorPromiseBase<CompositeAggregationSourcesDescriptor<T>, IList<ICompositeAggregationSource>>
    where T : class
  {
    public CompositeAggregationSourcesDescriptor()
      : base((IList<ICompositeAggregationSource>) new List<ICompositeAggregationSource>())
    {
    }

    public CompositeAggregationSourcesDescriptor<T> Terms(
      string name,
      Func<TermsCompositeAggregationSourceDescriptor<T>, ITermsCompositeAggregationSource> selector)
    {
      return this.Assign<ITermsCompositeAggregationSource>(selector != null ? selector(new TermsCompositeAggregationSourceDescriptor<T>(name)) : (ITermsCompositeAggregationSource) null, (Action<IList<ICompositeAggregationSource>, ITermsCompositeAggregationSource>) ((a, v) => a.Add((ICompositeAggregationSource) v)));
    }

    public CompositeAggregationSourcesDescriptor<T> Histogram(
      string name,
      Func<HistogramCompositeAggregationSourceDescriptor<T>, IHistogramCompositeAggregationSource> selector)
    {
      return this.Assign<IHistogramCompositeAggregationSource>(selector != null ? selector(new HistogramCompositeAggregationSourceDescriptor<T>(name)) : (IHistogramCompositeAggregationSource) null, (Action<IList<ICompositeAggregationSource>, IHistogramCompositeAggregationSource>) ((a, v) => a.Add((ICompositeAggregationSource) v)));
    }

    public CompositeAggregationSourcesDescriptor<T> DateHistogram(
      string name,
      Func<DateHistogramCompositeAggregationSourceDescriptor<T>, IDateHistogramCompositeAggregationSource> selector)
    {
      return this.Assign<IDateHistogramCompositeAggregationSource>(selector != null ? selector(new DateHistogramCompositeAggregationSourceDescriptor<T>(name)) : (IDateHistogramCompositeAggregationSource) null, (Action<IList<ICompositeAggregationSource>, IDateHistogramCompositeAggregationSource>) ((a, v) => a.Add((ICompositeAggregationSource) v)));
    }

    public CompositeAggregationSourcesDescriptor<T> GeoTileGrid(
      string name,
      Func<GeoTileGridCompositeAggregationSourceDescriptor<T>, IGeoTileGridCompositeAggregationSource> selector)
    {
      return this.Assign<IGeoTileGridCompositeAggregationSource>(selector != null ? selector(new GeoTileGridCompositeAggregationSourceDescriptor<T>(name)) : (IGeoTileGridCompositeAggregationSource) null, (Action<IList<ICompositeAggregationSource>, IGeoTileGridCompositeAggregationSource>) ((a, v) => a.Add((ICompositeAggregationSource) v)));
    }
  }
}
