// Decompiled with JetBrains decompiler
// Type: Nest.DetectorsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class DetectorsDescriptor<T> : 
    DescriptorPromiseBase<DetectorsDescriptor<T>, IList<IDetector>>
    where T : class
  {
    public DetectorsDescriptor()
      : base((IList<IDetector>) new List<IDetector>())
    {
    }

    public DetectorsDescriptor<T> Count(
      Func<CountDetectorDescriptor<T>, ICountDetector> selector = null)
    {
      return this.Assign<Func<CountDetectorDescriptor<T>, ICountDetector>>(selector, (Action<IList<IDetector>, Func<CountDetectorDescriptor<T>, ICountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<CountDetectorDescriptor<T>, ICountDetector>(new CountDetectorDescriptor<T>(CountFunction.Count)))));
    }

    public DetectorsDescriptor<T> HighCount(
      Func<CountDetectorDescriptor<T>, ICountDetector> selector = null)
    {
      return this.Assign<Func<CountDetectorDescriptor<T>, ICountDetector>>(selector, (Action<IList<IDetector>, Func<CountDetectorDescriptor<T>, ICountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<CountDetectorDescriptor<T>, ICountDetector>(new CountDetectorDescriptor<T>(CountFunction.HighCount)))));
    }

    public DetectorsDescriptor<T> LowCount(
      Func<CountDetectorDescriptor<T>, ICountDetector> selector = null)
    {
      return this.Assign<Func<CountDetectorDescriptor<T>, ICountDetector>>(selector, (Action<IList<IDetector>, Func<CountDetectorDescriptor<T>, ICountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<CountDetectorDescriptor<T>, ICountDetector>(new CountDetectorDescriptor<T>(CountFunction.LowCount)))));
    }

    public DetectorsDescriptor<T> NonZeroCount(
      Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector> selector = null)
    {
      return this.Assign<Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>>(selector, (Action<IList<IDetector>, Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>(new NonZeroCountDetectorDescriptor<T>(NonZeroCountFunction.NonZeroCount)))));
    }

    public DetectorsDescriptor<T> HighNonZeroCount(
      Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector> selector = null)
    {
      return this.Assign<Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>>(selector, (Action<IList<IDetector>, Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>(new NonZeroCountDetectorDescriptor<T>(NonZeroCountFunction.HighNonZeroCount)))));
    }

    public DetectorsDescriptor<T> LowNonZeroCount(
      Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector> selector = null)
    {
      return this.Assign<Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>>(selector, (Action<IList<IDetector>, Func<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<NonZeroCountDetectorDescriptor<T>, INonZeroCountDetector>(new NonZeroCountDetectorDescriptor<T>(NonZeroCountFunction.LowNonZeroCount)))));
    }

    public DetectorsDescriptor<T> DistinctCount(
      Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector> selector = null)
    {
      return this.Assign<Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>>(selector, (Action<IList<IDetector>, Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>(new DistinctCountDetectorDescriptor<T>(DistinctCountFunction.DistinctCount)))));
    }

    public DetectorsDescriptor<T> HighDistinctCount(
      Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector> selector = null)
    {
      return this.Assign<Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>>(selector, (Action<IList<IDetector>, Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>(new DistinctCountDetectorDescriptor<T>(DistinctCountFunction.HighDistinctCount)))));
    }

    public DetectorsDescriptor<T> LowDistinctCount(
      Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector> selector = null)
    {
      return this.Assign<Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>>(selector, (Action<IList<IDetector>, Func<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<DistinctCountDetectorDescriptor<T>, IDistinctCountDetector>(new DistinctCountDetectorDescriptor<T>(DistinctCountFunction.LowDistinctCount)))));
    }

    public DetectorsDescriptor<T> InfoContent(
      Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector> selector = null)
    {
      return this.Assign<Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector>>(selector, (Action<IList<IDetector>, Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<InfoContentDetectorDescriptor<T>, IInfoContentDetector>(new InfoContentDetectorDescriptor<T>(InfoContentFunction.InfoContent)))));
    }

    public DetectorsDescriptor<T> HighInfoContent(
      Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector> selector = null)
    {
      return this.Assign<Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector>>(selector, (Action<IList<IDetector>, Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<InfoContentDetectorDescriptor<T>, IInfoContentDetector>(new InfoContentDetectorDescriptor<T>(InfoContentFunction.HighInfoContent)))));
    }

    public DetectorsDescriptor<T> LowInfoContent(
      Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector> selector = null)
    {
      return this.Assign<Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector>>(selector, (Action<IList<IDetector>, Func<InfoContentDetectorDescriptor<T>, IInfoContentDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<InfoContentDetectorDescriptor<T>, IInfoContentDetector>(new InfoContentDetectorDescriptor<T>(InfoContentFunction.LowInfoContent)))));
    }

    public DetectorsDescriptor<T> Min(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.Min)))));
    }

    public DetectorsDescriptor<T> Max(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.Max)))));
    }

    public DetectorsDescriptor<T> Median(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.Median)))));
    }

    public DetectorsDescriptor<T> HighMedian(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.HighMedian)))));
    }

    public DetectorsDescriptor<T> LowMedian(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.LowMedian)))));
    }

    public DetectorsDescriptor<T> Mean(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.Mean)))));
    }

    public DetectorsDescriptor<T> HighMean(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.HighMean)))));
    }

    public DetectorsDescriptor<T> LowMean(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.LowMean)))));
    }

    public DetectorsDescriptor<T> Metric(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.Metric)))));
    }

    public DetectorsDescriptor<T> Varp(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.Varp)))));
    }

    public DetectorsDescriptor<T> HighVarp(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.HighVarp)))));
    }

    public DetectorsDescriptor<T> LowVarp(
      Func<MetricDetectorDescriptor<T>, IMetricDetector> selector = null)
    {
      return this.Assign<Func<MetricDetectorDescriptor<T>, IMetricDetector>>(selector, (Action<IList<IDetector>, Func<MetricDetectorDescriptor<T>, IMetricDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<MetricDetectorDescriptor<T>, IMetricDetector>(new MetricDetectorDescriptor<T>(MetricFunction.LowVarp)))));
    }

    public DetectorsDescriptor<T> Rare(
      Func<RareDetectorDescriptor<T>, IRareDetector> selector = null)
    {
      return this.Assign<Func<RareDetectorDescriptor<T>, IRareDetector>>(selector, (Action<IList<IDetector>, Func<RareDetectorDescriptor<T>, IRareDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<RareDetectorDescriptor<T>, IRareDetector>(new RareDetectorDescriptor<T>(RareFunction.Rare)))));
    }

    public DetectorsDescriptor<T> FreqRare(
      Func<RareDetectorDescriptor<T>, IRareDetector> selector = null)
    {
      return this.Assign<Func<RareDetectorDescriptor<T>, IRareDetector>>(selector, (Action<IList<IDetector>, Func<RareDetectorDescriptor<T>, IRareDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<RareDetectorDescriptor<T>, IRareDetector>(new RareDetectorDescriptor<T>(RareFunction.FreqRare)))));
    }

    public DetectorsDescriptor<T> Sum(
      Func<SumDetectorDescriptor<T>, ISumDetector> selector = null)
    {
      return this.Assign<Func<SumDetectorDescriptor<T>, ISumDetector>>(selector, (Action<IList<IDetector>, Func<SumDetectorDescriptor<T>, ISumDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<SumDetectorDescriptor<T>, ISumDetector>(new SumDetectorDescriptor<T>(SumFunction.Sum)))));
    }

    public DetectorsDescriptor<T> HighSum(
      Func<SumDetectorDescriptor<T>, ISumDetector> selector = null)
    {
      return this.Assign<Func<SumDetectorDescriptor<T>, ISumDetector>>(selector, (Action<IList<IDetector>, Func<SumDetectorDescriptor<T>, ISumDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<SumDetectorDescriptor<T>, ISumDetector>(new SumDetectorDescriptor<T>(SumFunction.HighSum)))));
    }

    public DetectorsDescriptor<T> LowSum(
      Func<SumDetectorDescriptor<T>, ISumDetector> selector = null)
    {
      return this.Assign<Func<SumDetectorDescriptor<T>, ISumDetector>>(selector, (Action<IList<IDetector>, Func<SumDetectorDescriptor<T>, ISumDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<SumDetectorDescriptor<T>, ISumDetector>(new SumDetectorDescriptor<T>(SumFunction.LowSum)))));
    }

    public DetectorsDescriptor<T> NonNullSum(
      Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector> selector = null)
    {
      return this.Assign<Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>>(selector, (Action<IList<IDetector>, Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>(new NonNullSumDetectorDescriptor<T>(NonNullSumFunction.NonNullSum)))));
    }

    public DetectorsDescriptor<T> HighNonNullSum(
      Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector> selector = null)
    {
      return this.Assign<Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>>(selector, (Action<IList<IDetector>, Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>(new NonNullSumDetectorDescriptor<T>(NonNullSumFunction.HighNonNullSum)))));
    }

    public DetectorsDescriptor<T> LowNonNullSum(
      Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector> selector = null)
    {
      return this.Assign<Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>>(selector, (Action<IList<IDetector>, Func<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<NonNullSumDetectorDescriptor<T>, INonNullSumDetector>(new NonNullSumDetectorDescriptor<T>(NonNullSumFunction.LowNonNullSum)))));
    }

    public DetectorsDescriptor<T> TimeOfDay(
      Func<TimeDetectorDescriptor<T>, ITimeDetector> selector = null)
    {
      return this.Assign<Func<TimeDetectorDescriptor<T>, ITimeDetector>>(selector, (Action<IList<IDetector>, Func<TimeDetectorDescriptor<T>, ITimeDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<TimeDetectorDescriptor<T>, ITimeDetector>(new TimeDetectorDescriptor<T>(TimeFunction.TimeOfDay)))));
    }

    public DetectorsDescriptor<T> TimeOfWeek(
      Func<TimeDetectorDescriptor<T>, ITimeDetector> selector = null)
    {
      return this.Assign<Func<TimeDetectorDescriptor<T>, ITimeDetector>>(selector, (Action<IList<IDetector>, Func<TimeDetectorDescriptor<T>, ITimeDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<TimeDetectorDescriptor<T>, ITimeDetector>(new TimeDetectorDescriptor<T>(TimeFunction.TimeOfWeek)))));
    }

    public DetectorsDescriptor<T> LatLong(
      Func<LatLongDetectorDescriptor<T>, IGeographicDetector> selector = null)
    {
      return this.Assign<Func<LatLongDetectorDescriptor<T>, IGeographicDetector>>(selector, (Action<IList<IDetector>, Func<LatLongDetectorDescriptor<T>, IGeographicDetector>>) ((a, v) => a.AddIfNotNull<IDetector>((IDetector) v.InvokeOrDefault<LatLongDetectorDescriptor<T>, IGeographicDetector>(new LatLongDetectorDescriptor<T>()))));
    }
  }
}
