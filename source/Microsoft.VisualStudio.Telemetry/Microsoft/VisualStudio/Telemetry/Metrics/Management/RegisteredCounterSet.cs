// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Management.RegisteredCounterSet
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Events;
using System;

namespace Microsoft.VisualStudio.Telemetry.Metrics.Management
{
  internal class RegisteredCounterSet : RegisteredMetricSetBase
  {
    internal RegisteredCounterSet()
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
        IVSCounter<T> vsCounter = meter.CreateVSCounter<T>(name, units, description);
        TelemetryCounterEvent<T> metricEvent1 = new TelemetryCounterEvent<T>(metricEvent, (ICounter<T>) vsCounter);
        return new RegisteredMetric(key, (TelemetryMetricEvent) metricEvent1);
      }
    }

    protected override void RecordIntegerData<T>(string key, T data, TimeSpan timeout)
    {
      RegisteredMetric integerMetric = this.IntegerMetrics[key];
      (integerMetric.MetricEvent.Metric as IVSCounter<long>).Add(Convert.ToInt64((object) data));
      integerMetric.UpdateExpirationTime(timeout);
    }

    protected override void RecordFloatingPointData<T>(string key, T data, TimeSpan timeout)
    {
      RegisteredMetric floatingPointMetric = this.FloatingPointMetrics[key];
      (floatingPointMetric.MetricEvent.Metric as IVSCounter<double>).Add(Convert.ToDouble((object) data));
      floatingPointMetric.UpdateExpirationTime(timeout);
    }
  }
}
