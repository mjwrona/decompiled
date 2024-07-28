// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Management.RegisteredHistogramSet
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Events;
using System;

namespace Microsoft.VisualStudio.Telemetry.Metrics.Management
{
  internal class RegisteredHistogramSet : RegisteredMetricSetBase
  {
    internal RegisteredHistogramSet()
    {
    }

    protected override RegisteredMetric CreateRegisteredMetricInstrument<T>(
      string key,
      string name,
      TelemetryEvent metricEvent,
      string units = null,
      string description = null,
      double[] buckets = null)
    {
      using (IMeter meter = new VSTelemetryMeterProvider().CreateMeter("Microsoft.VisualStudio.RegisteredMetrics", "1.0"))
      {
        HistogramConfiguration configuration = new HistogramConfiguration(buckets);
        IVSHistogram<T> vsHistogram = meter.CreateVSHistogram<T>(name, configuration, units, description);
        TelemetryHistogramEvent<T> metricEvent1 = new TelemetryHistogramEvent<T>(metricEvent, (IHistogram<T>) vsHistogram);
        return new RegisteredMetric(key, (TelemetryMetricEvent) metricEvent1);
      }
    }

    protected override void RecordIntegerData<T>(string key, T data, TimeSpan timeout)
    {
      RegisteredMetric integerMetric = this.IntegerMetrics[key];
      (integerMetric.MetricEvent.Metric as IVSHistogram<long>).Record(Convert.ToInt64((object) data));
      integerMetric.UpdateExpirationTime(timeout);
    }

    protected override void RecordFloatingPointData<T>(string key, T data, TimeSpan timeout)
    {
      RegisteredMetric floatingPointMetric = this.FloatingPointMetrics[key];
      (floatingPointMetric.MetricEvent.Metric as IVSHistogram<double>).Record(Convert.ToDouble((object) data));
      floatingPointMetric.UpdateExpirationTime(timeout);
    }
  }
}
