// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.HistogramStatistics`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public class HistogramStatistics<T> where T : struct
  {
    private readonly HistogramConfiguration configuration;
    private readonly SortedList<T, T> dataPoints;

    public IVSCounter<T> Counter { get; private set; }

    public T? Min { get; private set; }

    public T? Max { get; private set; }

    public double? Average => GenericNumericUtility<T>.Average(this.Counter.Sum, this.Counter.Count);

    public double? Median => this.CalculateMedian();

    public DateTime? FirstRecorded { get; private set; }

    public DateTime? LastRecorded { get; private set; }

    public HistogramStatistics(IMeter meter, HistogramConfiguration configuration)
    {
      this.configuration = configuration;
      this.Counter = meter.CreateVSUpDownCounter<T>("Bucket Count");
      this.dataPoints = this.configuration.RecordMedian ? new SortedList<T, T>((IComparer<T>) new DataPointEqualityComparer<T>()) : (SortedList<T, T>) null;
    }

    internal void Record(T measurement)
    {
      this.Counter.Add(measurement);
      if (this.configuration.RecordMinMax)
      {
        T? nullable = this.Min;
        if (!nullable.HasValue)
          this.Min = new T?(measurement);
        nullable = this.Max;
        if (!nullable.HasValue)
          this.Max = new T?(measurement);
        Comparer<T> comparer = Comparer<T>.Default;
        T x1 = measurement;
        nullable = this.Min;
        T y1 = nullable.Value;
        if (comparer.Compare(x1, y1) < 0)
          this.Min = new T?(measurement);
        T x2 = measurement;
        nullable = this.Max;
        T y2 = nullable.Value;
        if (comparer.Compare(x2, y2) > 0)
          this.Max = new T?(measurement);
      }
      if (this.dataPoints != null)
        this.dataPoints.Add(measurement, measurement);
      DateTime utcNow = DateTime.UtcNow;
      this.LastRecorded = new DateTime?(utcNow);
      if (this.FirstRecorded.HasValue)
        return;
      this.FirstRecorded = new DateTime?(utcNow);
    }

    internal double? CalculateMedian()
    {
      if (this.dataPoints == null || this.dataPoints.Count == 0)
        return new double?();
      if (this.dataPoints.Count % 2 == 1)
        return new double?(Convert.ToDouble((object) this.dataPoints.ElementAt<KeyValuePair<T, T>>((this.dataPoints.Count - 1) / 2).Value));
      int index = this.dataPoints.Count / 2;
      return GenericNumericUtility<T>.Average(GenericNumericUtility<T>.Add(this.dataPoints.ElementAt<KeyValuePair<T, T>>(index - 1).Value, this.dataPoints.ElementAt<KeyValuePair<T, T>>(index).Value), 2L);
    }
  }
}
