// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.MetricInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram;
using Microsoft.Azure.Cosmos.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [Serializable]
  internal sealed class MetricInfo
  {
    internal MetricInfo(string metricsName, string unitName)
    {
      this.MetricsName = metricsName;
      this.UnitName = unitName;
    }

    public MetricInfo(
      string metricsName,
      string unitName,
      double mean = 0.0,
      long count = 0,
      long min = 0,
      long max = 0,
      IReadOnlyDictionary<double, double> percentiles = null)
      : this(metricsName, unitName)
    {
      this.Mean = mean;
      this.Count = count;
      this.Min = (double) min;
      this.Max = (double) max;
      this.Percentiles = percentiles;
    }

    [JsonProperty(PropertyName = "metricsName")]
    internal string MetricsName { get; }

    [JsonProperty(PropertyName = "unitName")]
    internal string UnitName { get; }

    [JsonProperty(PropertyName = "mean")]
    internal double Mean { get; set; }

    [JsonProperty(PropertyName = "count")]
    internal long Count { get; set; }

    [JsonProperty(PropertyName = "min")]
    internal double Min { get; set; }

    [JsonProperty(PropertyName = "max")]
    internal double Max { get; set; }

    [JsonProperty(PropertyName = "percentiles")]
    internal IReadOnlyDictionary<double, double> Percentiles { get; set; }

    internal MetricInfo SetAggregators(LongConcurrentHistogram histogram, double adjustment = 1.0)
    {
      if (histogram != null)
      {
        this.Count = histogram.TotalCount;
        this.Max = (double) histogram.GetMaxValue() / adjustment;
        this.Min = (double) histogram.GetMinValue() / adjustment;
        this.Mean = histogram.GetMean() / adjustment;
        this.Percentiles = (IReadOnlyDictionary<double, double>) new Dictionary<double, double>()
        {
          {
            50.0,
            (double) histogram.GetValueAtPercentile(50.0) / adjustment
          },
          {
            90.0,
            (double) histogram.GetValueAtPercentile(90.0) / adjustment
          },
          {
            95.0,
            (double) histogram.GetValueAtPercentile(95.0) / adjustment
          },
          {
            99.0,
            (double) histogram.GetValueAtPercentile(99.0) / adjustment
          },
          {
            99.9,
            (double) histogram.GetValueAtPercentile(99.9) / adjustment
          }
        };
      }
      return this;
    }
  }
}
