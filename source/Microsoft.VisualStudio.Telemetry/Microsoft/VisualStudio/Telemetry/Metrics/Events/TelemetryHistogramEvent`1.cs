// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Events.TelemetryHistogramEvent`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Metrics.Events
{
  public class TelemetryHistogramEvent<T> : TelemetryMetricEvent where T : struct
  {
    private const string HistogramPrefix = "Histogram.";

    private IVSHistogram<T> Histogram { get; set; }

    public TelemetryHistogramEvent(TelemetryEvent telemetryEvent, IHistogram<T> histogram)
      : base(telemetryEvent, (IInstrument) histogram)
    {
      if (!(histogram is IVSHistogram<T>))
        return;
      this.Histogram = histogram as IVSHistogram<T>;
    }

    protected override void SetMetricProperties()
    {
      if (this.Histogram == null)
        return;
      List<object> values1 = new List<object>();
      List<object> values2 = new List<object>();
      List<object> values3 = new List<object>();
      List<object> values4 = new List<object>();
      List<object> values5 = new List<object>();
      List<object> values6 = new List<object>();
      List<object> values7 = new List<object>();
      List<object> values8 = new List<object>();
      T? nullable1;
      foreach (HistogramBucket<T> bucket in this.Histogram.Buckets.Buckets)
      {
        double num;
        string str1;
        if (bucket.MinBoundary != double.NegativeInfinity)
        {
          num = bucket.MinBoundary;
          str1 = num.ToString();
        }
        else
          str1 = "-Infinity";
        string str2 = str1;
        string str3;
        if (bucket.MaxBoundary != double.PositiveInfinity)
        {
          num = bucket.MaxBoundary;
          str3 = num.ToString();
        }
        else
          str3 = "Infinity";
        string str4 = str3;
        values1.Add((object) str2);
        values2.Add((object) (str2 + "-" + str4));
        values3.Add((object) bucket.Statistics.Counter.Sum);
        values4.Add((object) bucket.Statistics.Counter.Count);
        values5.Add((object) bucket.Statistics.Average.GetValueOrDefault());
        List<object> objectList1 = values6;
        nullable1 = bucket.Statistics.Min;
        // ISSUE: variable of a boxed type
        __Boxed<T> valueOrDefault1 = (ValueType) nullable1.GetValueOrDefault();
        objectList1.Add((object) valueOrDefault1);
        List<object> objectList2 = values7;
        nullable1 = bucket.Statistics.Max;
        // ISSUE: variable of a boxed type
        __Boxed<T> valueOrDefault2 = (ValueType) nullable1.GetValueOrDefault();
        objectList2.Add((object) valueOrDefault2);
        values8.Add((object) bucket.Statistics.Median.GetValueOrDefault());
      }
      this.SetCustomMetricProperty("Histogram.Summary.BucketRanges", (object) string.Join<object>(",", (IEnumerable<object>) values1));
      this.SetCustomMetricProperty("Histogram.Summary.FriendlyBucketRanges", (object) string.Join<object>(",", (IEnumerable<object>) values2));
      this.SetCustomMetricProperty("Histogram.Summary.Sum", (object) this.Histogram.Statistics.Counter.Sum);
      this.SetCustomMetricProperty("Histogram.Summary.Count", (object) this.Histogram.Statistics.Counter.Count);
      this.SetCustomMetricProperty("Histogram.Summary.Average", (object) this.Histogram.Statistics.Average);
      this.SetCustomMetricProperty("Histogram.Buckets.Sums", (object) string.Join<object>(",", (IEnumerable<object>) values3));
      this.SetCustomMetricProperty("Histogram.Buckets.Counts", (object) string.Join<object>(",", (IEnumerable<object>) values4));
      this.SetCustomMetricProperty("Histogram.Buckets.Averages", (object) string.Join<object>(",", (IEnumerable<object>) values5));
      nullable1 = this.Histogram.Statistics.Min;
      if (nullable1.HasValue)
      {
        this.SetCustomMetricProperty("Histogram.Summary.Min", (object) this.Histogram.Statistics.Min);
        this.SetCustomMetricProperty("Histogram.Buckets.Mins", (object) string.Join<object>(",", (IEnumerable<object>) values6));
      }
      nullable1 = this.Histogram.Statistics.Max;
      if (nullable1.HasValue)
      {
        this.SetCustomMetricProperty("Histogram.Summary.Max", (object) this.Histogram.Statistics.Max);
        this.SetCustomMetricProperty("Histogram.Buckets.Maxes", (object) string.Join<object>(",", (IEnumerable<object>) values7));
      }
      if (this.Histogram.Statistics.Median.HasValue)
      {
        this.SetCustomMetricProperty("Histogram.Summary.Median", (object) this.Histogram.Statistics.Median);
        this.SetCustomMetricProperty("Histogram.Buckets.Medians", (object) string.Join<object>(",", (IEnumerable<object>) values8));
      }
      DateTime? nullable2 = this.Histogram.Statistics.FirstRecorded;
      if (nullable2.HasValue)
      {
        nullable2 = this.Histogram.Statistics.FirstRecorded;
        this.SetCustomMetricProperty("Histogram.Summary.FirstRecorded", (object) nullable2.ToString());
      }
      nullable2 = this.Histogram.Statistics.LastRecorded;
      if (!nullable2.HasValue)
        return;
      this.SetCustomMetricProperty("Histogram.Summary.LastRecorded", (object) this.Histogram.Statistics.LastRecorded);
    }
  }
}
