// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.HistogramConfiguration
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Exceptions;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public class HistogramConfiguration
  {
    private readonly double[] explicitBuckets;
    private readonly bool recordMinMax;
    private readonly bool recordMedian;
    internal static readonly double[] DefaultHistogramBuckets = new double[15]
    {
      0.0,
      5.0,
      10.0,
      25.0,
      50.0,
      75.0,
      100.0,
      250.0,
      500.0,
      750.0,
      1000.0,
      2500.0,
      5000.0,
      7500.0,
      10000.0
    };

    internal double[] ExplicitBuckets => this.explicitBuckets;

    internal bool RecordMinMax => this.recordMinMax;

    internal bool RecordMedian => this.recordMedian;

    public HistogramConfiguration(
      double[] explicitBucketBoundaries = null,
      bool recordMinMax = false,
      bool recordMedian = false)
    {
      this.explicitBuckets = explicitBucketBoundaries ?? HistogramConfiguration.DefaultHistogramBuckets;
      this.recordMinMax = recordMinMax;
      this.recordMedian = recordMedian;
      if (explicitBucketBoundaries == null)
        return;
      this.ValidateBucketOrdering();
    }

    private void ValidateBucketOrdering()
    {
      double? nullable = new double?();
      for (int index = 0; index < this.ExplicitBuckets.Length; ++index)
      {
        double explicitBucket = this.ExplicitBuckets[index];
        if (nullable.HasValue && nullable.Value > explicitBucket)
          throw new InvalidBucketConfigurationException();
        nullable = new double?(explicitBucket);
      }
    }
  }
}
