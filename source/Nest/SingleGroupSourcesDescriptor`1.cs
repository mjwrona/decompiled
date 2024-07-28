// Decompiled with JetBrains decompiler
// Type: Nest.SingleGroupSourcesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SingleGroupSourcesDescriptor<T> : 
    DescriptorPromiseBase<SingleGroupSourcesDescriptor<T>, IDictionary<string, ISingleGroupSource>>
    where T : class
  {
    public SingleGroupSourcesDescriptor()
      : base((IDictionary<string, ISingleGroupSource>) new Dictionary<string, ISingleGroupSource>())
    {
    }

    public SingleGroupSourcesDescriptor<T> Terms(
      string name,
      Func<TermsGroupSourceDescriptor<T>, ITermsGroupSource> selector)
    {
      return this.Assign<Tuple<string, ITermsGroupSource>>(new Tuple<string, ITermsGroupSource>(name, selector != null ? selector(new TermsGroupSourceDescriptor<T>()) : (ITermsGroupSource) null), (Action<IDictionary<string, ISingleGroupSource>, Tuple<string, ITermsGroupSource>>) ((a, v) => a.Add(v.Item1, (ISingleGroupSource) v.Item2)));
    }

    public SingleGroupSourcesDescriptor<T> Histogram(
      string name,
      Func<HistogramGroupSourceDescriptor<T>, IHistogramGroupSource> selector)
    {
      return this.Assign<Tuple<string, IHistogramGroupSource>>(new Tuple<string, IHistogramGroupSource>(name, selector != null ? selector(new HistogramGroupSourceDescriptor<T>()) : (IHistogramGroupSource) null), (Action<IDictionary<string, ISingleGroupSource>, Tuple<string, IHistogramGroupSource>>) ((a, v) => a.Add(v.Item1, (ISingleGroupSource) v.Item2)));
    }

    public SingleGroupSourcesDescriptor<T> DateHistogram(
      string name,
      Func<DateHistogramGroupSourceDescriptor<T>, IDateHistogramGroupSource> selector)
    {
      return this.Assign<Tuple<string, IDateHistogramGroupSource>>(new Tuple<string, IDateHistogramGroupSource>(name, selector != null ? selector(new DateHistogramGroupSourceDescriptor<T>()) : (IDateHistogramGroupSource) null), (Action<IDictionary<string, ISingleGroupSource>, Tuple<string, IDateHistogramGroupSource>>) ((a, v) => a.Add(v.Item1, (ISingleGroupSource) v.Item2)));
    }

    public SingleGroupSourcesDescriptor<T> GeoTileGrid(
      string name,
      Func<GeoTileGridGroupSourceDescriptor<T>, IGeoTileGridGroupSource> selector)
    {
      return this.Assign<Tuple<string, IGeoTileGridGroupSource>>(new Tuple<string, IGeoTileGridGroupSource>(name, selector != null ? selector(new GeoTileGridGroupSourceDescriptor<T>()) : (IGeoTileGridGroupSource) null), (Action<IDictionary<string, ISingleGroupSource>, Tuple<string, IGeoTileGridGroupSource>>) ((a, v) => a.Add(v.Item1, (ISingleGroupSource) v.Item2)));
    }
  }
}
