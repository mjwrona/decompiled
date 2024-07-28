// Decompiled with JetBrains decompiler
// Type: Nest.MetricFunctionsExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class MetricFunctionsExtensions
  {
    public static string GetStringValue(this MetricFunction metricFunction)
    {
      switch (metricFunction)
      {
        case MetricFunction.Min:
          return "min";
        case MetricFunction.Max:
          return "max";
        case MetricFunction.Median:
          return "median";
        case MetricFunction.HighMedian:
          return "high_median";
        case MetricFunction.LowMedian:
          return "low_median";
        case MetricFunction.Mean:
          return "mean";
        case MetricFunction.HighMean:
          return "high_mean";
        case MetricFunction.LowMean:
          return "low_mean";
        case MetricFunction.Metric:
          return "metric";
        case MetricFunction.Varp:
          return "varp";
        case MetricFunction.HighVarp:
          return "high_varp";
        case MetricFunction.LowVarp:
          return "low_varp";
        default:
          throw new ArgumentOutOfRangeException(nameof (metricFunction), (object) metricFunction, (string) null);
      }
    }
  }
}
