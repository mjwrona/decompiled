// Decompiled with JetBrains decompiler
// Type: Nest.ValidateDetectorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class ValidateDetectorDescriptor<TDocument> : 
    RequestDescriptorBase<ValidateDetectorDescriptor<TDocument>, ValidateDetectorRequestParameters, IValidateDetectorRequest>,
    IValidateDetectorRequest,
    IRequest<ValidateDetectorRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningValidateDetector;

    IDetector IValidateDetectorRequest.Detector { get; set; }

    public ValidateDetectorDescriptor<TDocument> Count(
      Func<CountDetectorDescriptor<TDocument>, ICountDetector> selector = null)
    {
      return this.Assign<ICountDetector>(selector.InvokeOrDefault<CountDetectorDescriptor<TDocument>, ICountDetector>(new CountDetectorDescriptor<TDocument>(CountFunction.Count)), (Action<IValidateDetectorRequest, ICountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighCount(
      Func<CountDetectorDescriptor<TDocument>, ICountDetector> selector = null)
    {
      return this.Assign<ICountDetector>(selector.InvokeOrDefault<CountDetectorDescriptor<TDocument>, ICountDetector>(new CountDetectorDescriptor<TDocument>(CountFunction.HighCount)), (Action<IValidateDetectorRequest, ICountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowCount(
      Func<CountDetectorDescriptor<TDocument>, ICountDetector> selector = null)
    {
      return this.Assign<ICountDetector>(selector.InvokeOrDefault<CountDetectorDescriptor<TDocument>, ICountDetector>(new CountDetectorDescriptor<TDocument>(CountFunction.LowCount)), (Action<IValidateDetectorRequest, ICountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> NonZeroCount(
      Func<NonZeroCountDetectorDescriptor<TDocument>, INonZeroCountDetector> selector = null)
    {
      return this.Assign<INonZeroCountDetector>(selector.InvokeOrDefault<NonZeroCountDetectorDescriptor<TDocument>, INonZeroCountDetector>(new NonZeroCountDetectorDescriptor<TDocument>(NonZeroCountFunction.NonZeroCount)), (Action<IValidateDetectorRequest, INonZeroCountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighNonZeroCount(
      Func<NonZeroCountDetectorDescriptor<TDocument>, INonZeroCountDetector> selector = null)
    {
      return this.Assign<INonZeroCountDetector>(selector.InvokeOrDefault<NonZeroCountDetectorDescriptor<TDocument>, INonZeroCountDetector>(new NonZeroCountDetectorDescriptor<TDocument>(NonZeroCountFunction.HighNonZeroCount)), (Action<IValidateDetectorRequest, INonZeroCountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowNonZeroCount(
      Func<NonZeroCountDetectorDescriptor<TDocument>, INonZeroCountDetector> selector = null)
    {
      return this.Assign<INonZeroCountDetector>(selector.InvokeOrDefault<NonZeroCountDetectorDescriptor<TDocument>, INonZeroCountDetector>(new NonZeroCountDetectorDescriptor<TDocument>(NonZeroCountFunction.LowNonZeroCount)), (Action<IValidateDetectorRequest, INonZeroCountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> DistinctCount(
      Func<DistinctCountDetectorDescriptor<TDocument>, IDistinctCountDetector> selector = null)
    {
      return this.Assign<IDistinctCountDetector>(selector.InvokeOrDefault<DistinctCountDetectorDescriptor<TDocument>, IDistinctCountDetector>(new DistinctCountDetectorDescriptor<TDocument>(DistinctCountFunction.DistinctCount)), (Action<IValidateDetectorRequest, IDistinctCountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighDistinctCount(
      Func<DistinctCountDetectorDescriptor<TDocument>, IDistinctCountDetector> selector = null)
    {
      return this.Assign<IDistinctCountDetector>(selector.InvokeOrDefault<DistinctCountDetectorDescriptor<TDocument>, IDistinctCountDetector>(new DistinctCountDetectorDescriptor<TDocument>(DistinctCountFunction.HighDistinctCount)), (Action<IValidateDetectorRequest, IDistinctCountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowDistinctCount(
      Func<DistinctCountDetectorDescriptor<TDocument>, IDistinctCountDetector> selector = null)
    {
      return this.Assign<IDistinctCountDetector>(selector.InvokeOrDefault<DistinctCountDetectorDescriptor<TDocument>, IDistinctCountDetector>(new DistinctCountDetectorDescriptor<TDocument>(DistinctCountFunction.LowDistinctCount)), (Action<IValidateDetectorRequest, IDistinctCountDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> InfoContent(
      Func<InfoContentDetectorDescriptor<TDocument>, IInfoContentDetector> selector = null)
    {
      return this.Assign<IInfoContentDetector>(selector.InvokeOrDefault<InfoContentDetectorDescriptor<TDocument>, IInfoContentDetector>(new InfoContentDetectorDescriptor<TDocument>(InfoContentFunction.InfoContent)), (Action<IValidateDetectorRequest, IInfoContentDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighInfoContent(
      Func<InfoContentDetectorDescriptor<TDocument>, IInfoContentDetector> selector = null)
    {
      return this.Assign<IInfoContentDetector>(selector.InvokeOrDefault<InfoContentDetectorDescriptor<TDocument>, IInfoContentDetector>(new InfoContentDetectorDescriptor<TDocument>(InfoContentFunction.HighInfoContent)), (Action<IValidateDetectorRequest, IInfoContentDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowInfoContent(
      Func<InfoContentDetectorDescriptor<TDocument>, IInfoContentDetector> selector = null)
    {
      return this.Assign<IInfoContentDetector>(selector.InvokeOrDefault<InfoContentDetectorDescriptor<TDocument>, IInfoContentDetector>(new InfoContentDetectorDescriptor<TDocument>(InfoContentFunction.LowInfoContent)), (Action<IValidateDetectorRequest, IInfoContentDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Min(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.Min)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Max(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.Max)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Median(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.Median)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighMedian(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.HighMedian)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowMedian(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.LowMedian)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Mean(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.Mean)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighMean(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.HighMean)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowMean(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.LowMean)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Metric(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.Metric)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Varp(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.Varp)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighVarp(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.HighVarp)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowVarp(
      Func<MetricDetectorDescriptor<TDocument>, IMetricDetector> selector = null)
    {
      return this.Assign<IMetricDetector>(selector.InvokeOrDefault<MetricDetectorDescriptor<TDocument>, IMetricDetector>(new MetricDetectorDescriptor<TDocument>(MetricFunction.LowVarp)), (Action<IValidateDetectorRequest, IMetricDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Rare(
      Func<RareDetectorDescriptor<TDocument>, IRareDetector> selector = null)
    {
      return this.Assign<IRareDetector>(selector.InvokeOrDefault<RareDetectorDescriptor<TDocument>, IRareDetector>(new RareDetectorDescriptor<TDocument>(RareFunction.Rare)), (Action<IValidateDetectorRequest, IRareDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> FreqRare(
      Func<RareDetectorDescriptor<TDocument>, IRareDetector> selector = null)
    {
      return this.Assign<IRareDetector>(selector.InvokeOrDefault<RareDetectorDescriptor<TDocument>, IRareDetector>(new RareDetectorDescriptor<TDocument>(RareFunction.FreqRare)), (Action<IValidateDetectorRequest, IRareDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> Sum(
      Func<SumDetectorDescriptor<TDocument>, ISumDetector> selector = null)
    {
      return this.Assign<ISumDetector>(selector.InvokeOrDefault<SumDetectorDescriptor<TDocument>, ISumDetector>(new SumDetectorDescriptor<TDocument>(SumFunction.Sum)), (Action<IValidateDetectorRequest, ISumDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighSum(
      Func<SumDetectorDescriptor<TDocument>, ISumDetector> selector = null)
    {
      return this.Assign<ISumDetector>(selector.InvokeOrDefault<SumDetectorDescriptor<TDocument>, ISumDetector>(new SumDetectorDescriptor<TDocument>(SumFunction.HighSum)), (Action<IValidateDetectorRequest, ISumDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowSum(
      Func<SumDetectorDescriptor<TDocument>, ISumDetector> selector = null)
    {
      return this.Assign<ISumDetector>(selector.InvokeOrDefault<SumDetectorDescriptor<TDocument>, ISumDetector>(new SumDetectorDescriptor<TDocument>(SumFunction.LowSum)), (Action<IValidateDetectorRequest, ISumDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> NonNullSum(
      Func<NonNullSumDetectorDescriptor<TDocument>, INonNullSumDetector> selector = null)
    {
      return this.Assign<INonNullSumDetector>(selector.InvokeOrDefault<NonNullSumDetectorDescriptor<TDocument>, INonNullSumDetector>(new NonNullSumDetectorDescriptor<TDocument>(NonNullSumFunction.NonNullSum)), (Action<IValidateDetectorRequest, INonNullSumDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> HighNonNullSum(
      Func<NonNullSumDetectorDescriptor<TDocument>, INonNullSumDetector> selector = null)
    {
      return this.Assign<INonNullSumDetector>(selector.InvokeOrDefault<NonNullSumDetectorDescriptor<TDocument>, INonNullSumDetector>(new NonNullSumDetectorDescriptor<TDocument>(NonNullSumFunction.HighNonNullSum)), (Action<IValidateDetectorRequest, INonNullSumDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> LowNonNullSum(
      Func<NonNullSumDetectorDescriptor<TDocument>, INonNullSumDetector> selector = null)
    {
      return this.Assign<INonNullSumDetector>(selector.InvokeOrDefault<NonNullSumDetectorDescriptor<TDocument>, INonNullSumDetector>(new NonNullSumDetectorDescriptor<TDocument>(NonNullSumFunction.LowNonNullSum)), (Action<IValidateDetectorRequest, INonNullSumDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> TimeOfDay(
      Func<TimeDetectorDescriptor<TDocument>, ITimeDetector> selector = null)
    {
      return this.Assign<ITimeDetector>(selector.InvokeOrDefault<TimeDetectorDescriptor<TDocument>, ITimeDetector>(new TimeDetectorDescriptor<TDocument>(TimeFunction.TimeOfDay)), (Action<IValidateDetectorRequest, ITimeDetector>) ((a, v) => a.Detector = (IDetector) v));
    }

    public ValidateDetectorDescriptor<TDocument> TimeOfWeek(
      Func<TimeDetectorDescriptor<TDocument>, ITimeDetector> selector = null)
    {
      return this.Assign<ITimeDetector>(selector.InvokeOrDefault<TimeDetectorDescriptor<TDocument>, ITimeDetector>(new TimeDetectorDescriptor<TDocument>(TimeFunction.TimeOfWeek)), (Action<IValidateDetectorRequest, ITimeDetector>) ((a, v) => a.Detector = (IDetector) v));
    }
  }
}
