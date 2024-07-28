// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Histogram`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  internal class Histogram<T> : Instrument<T>, IHistogram<T>, IInstrument, IVSHistogram<T> where T : struct
  {
    private HistogramConfiguration configuration;

    public HistogramStatistics<T> Statistics { get; private set; }

    public HistogramBuckets<T> Buckets { get; private set; }

    private HistogramConfiguration Configuration => this.configuration;

    internal Histogram(IMeter meter, string name, string unit = null, string description = null)
      : base(meter, name, unit, description)
    {
      this.configuration = new HistogramConfiguration();
      this.Statistics = new HistogramStatistics<T>(meter, this.configuration);
      this.Buckets = new HistogramBuckets<T>(meter, this.configuration);
    }

    internal Histogram(
      IMeter meter,
      string name,
      HistogramConfiguration configuration,
      string unit = null,
      string description = null)
      : base(meter, name, unit, description)
    {
      this.configuration = configuration;
      this.Statistics = new HistogramStatistics<T>(meter, this.configuration);
      this.Buckets = new HistogramBuckets<T>(meter, this.configuration);
    }

    public void Record(T value) => this.RecordMeasurement(value);

    public void Record(T value, KeyValuePair<string, object> tag) => this.RecordMeasurement(value, tag);

    public void Record(
      T value,
      KeyValuePair<string, object> tag1,
      KeyValuePair<string, object> tag2)
    {
      this.RecordMeasurement(value, tag1, tag2);
    }

    public void Record(
      T value,
      KeyValuePair<string, object> tag1,
      KeyValuePair<string, object> tag2,
      KeyValuePair<string, object> tag3)
    {
      this.RecordMeasurement(value, tag1, tag2, tag3);
    }

    public void Record(T value, params KeyValuePair<string, object>[] tags) => this.RecordMeasurement(value, (ReadOnlySpan<KeyValuePair<string, object>>) tags.AsSpan<KeyValuePair<string, object>>());

    public void Record(T value, ReadOnlySpan<KeyValuePair<string, object>> tags) => this.RecordMeasurement(value, tags);

    protected override void RecordMeasurement(
      T measurement,
      ReadOnlySpan<KeyValuePair<string, object>> tags)
    {
      this.Statistics.Record(measurement);
      this.Buckets.Record(measurement);
    }
  }
}
