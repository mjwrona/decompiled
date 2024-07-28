// Decompiled with JetBrains decompiler
// Type: Nest.RollupGroupingsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RollupGroupingsDescriptor<T> : 
    DescriptorBase<RollupGroupingsDescriptor<T>, IRollupGroupings>,
    IRollupGroupings
    where T : class
  {
    IDateHistogramRollupGrouping IRollupGroupings.DateHistogram { get; set; }

    IHistogramRollupGrouping IRollupGroupings.Histogram { get; set; }

    ITermsRollupGrouping IRollupGroupings.Terms { get; set; }

    public RollupGroupingsDescriptor<T> DateHistogram(
      Func<DateHistogramRollupGroupingDescriptor<T>, IDateHistogramRollupGrouping> selector)
    {
      return this.Assign<Func<DateHistogramRollupGroupingDescriptor<T>, IDateHistogramRollupGrouping>>(selector, (Action<IRollupGroupings, Func<DateHistogramRollupGroupingDescriptor<T>, IDateHistogramRollupGrouping>>) ((a, v) => a.DateHistogram = v != null ? v(new DateHistogramRollupGroupingDescriptor<T>()) : (IDateHistogramRollupGrouping) null));
    }

    public RollupGroupingsDescriptor<T> Histogram(
      Func<HistogramRollupGroupingDescriptor<T>, IHistogramRollupGrouping> selector)
    {
      return this.Assign<Func<HistogramRollupGroupingDescriptor<T>, IHistogramRollupGrouping>>(selector, (Action<IRollupGroupings, Func<HistogramRollupGroupingDescriptor<T>, IHistogramRollupGrouping>>) ((a, v) => a.Histogram = v != null ? v(new HistogramRollupGroupingDescriptor<T>()) : (IHistogramRollupGrouping) null));
    }

    public RollupGroupingsDescriptor<T> Terms(
      Func<TermsRollupGroupingDescriptor<T>, ITermsRollupGrouping> selector)
    {
      return this.Assign<Func<TermsRollupGroupingDescriptor<T>, ITermsRollupGrouping>>(selector, (Action<IRollupGroupings, Func<TermsRollupGroupingDescriptor<T>, ITermsRollupGrouping>>) ((a, v) => a.Terms = v != null ? v(new TermsRollupGroupingDescriptor<T>()) : (ITermsRollupGrouping) null));
    }
  }
}
