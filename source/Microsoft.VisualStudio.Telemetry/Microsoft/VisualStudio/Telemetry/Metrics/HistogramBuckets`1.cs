// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.HistogramBuckets`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public class HistogramBuckets<T> where T : struct
  {
    internal HistogramBucket<T>[] Buckets { get; private set; }

    public HistogramBuckets(IMeter meter, HistogramConfiguration configuration) => this.CreateBuckets(meter, configuration);

    internal void Record(T measurement)
    {
      foreach (HistogramBucket<T> bucket in this.Buckets)
      {
        if (bucket.IsCorrectBucket(measurement))
        {
          bucket.Statistics.Record(measurement);
          break;
        }
      }
    }

    private void CreateBuckets(IMeter meter, HistogramConfiguration configuration)
    {
      this.Buckets = new HistogramBucket<T>[configuration.ExplicitBuckets.Length + 1];
      double num = double.NegativeInfinity;
      for (int index = 0; index < configuration.ExplicitBuckets.Length; ++index)
      {
        double minBoundary = num;
        num = configuration.ExplicitBuckets[index];
        this.Buckets[index] = new HistogramBucket<T>(minBoundary, num, meter, configuration);
      }
      this.Buckets[configuration.ExplicitBuckets.Length] = new HistogramBucket<T>(num, double.PositiveInfinity, meter, configuration);
    }
  }
}
