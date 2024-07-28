// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Management.RegisteredMetricSetBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Exceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Metrics.Management
{
  internal abstract class RegisteredMetricSetBase
  {
    protected const string RegisteredMetricMeterName = "Microsoft.VisualStudio.RegisteredMetrics";
    protected const string RegisteredMetricMeterVersion = "1.0";

    internal Dictionary<string, RegisteredMetric> IntegerMetrics { get; set; }

    internal Dictionary<string, RegisteredMetric> FloatingPointMetrics { get; set; }

    internal int ConcurrentMetricsCount => this.IntegerMetrics.Count + this.FloatingPointMetrics.Count;

    internal RegisteredMetricSetBase()
    {
      this.IntegerMetrics = new Dictionary<string, RegisteredMetric>();
      this.FloatingPointMetrics = new Dictionary<string, RegisteredMetric>();
    }

    internal void Record<T>(
      string key,
      T data,
      string metricName,
      TelemetryEvent metricEvent,
      TimeSpan timeout,
      double[] buckets = null)
      where T : struct
    {
      switch (this.DetermineMetricDataType(typeof (T)))
      {
        case MetricDataType.Integer:
          if (!this.IntegerMetrics.ContainsKey(key))
          {
            RegisteredMetric metricInstrument = this.CreateRegisteredMetricInstrument<long>(key, metricName, metricEvent, buckets: buckets);
            this.IntegerMetrics.Add(key, metricInstrument);
          }
          this.RecordIntegerData<T>(key, data, timeout);
          break;
        case MetricDataType.FloatingPoint:
          if (!this.FloatingPointMetrics.ContainsKey(key))
          {
            RegisteredMetric metricInstrument = this.CreateRegisteredMetricInstrument<double>(key, metricName, metricEvent, buckets: buckets);
            this.FloatingPointMetrics.Add(key, metricInstrument);
          }
          this.RecordFloatingPointData<T>(key, data, timeout);
          break;
        default:
          throw new UnsupportedNumericStructException();
      }
    }

    protected abstract RegisteredMetric CreateRegisteredMetricInstrument<T>(
      string key,
      string name,
      TelemetryEvent metricEvent,
      string units = null,
      string description = null,
      double[] buckets = null)
      where T : struct;

    protected abstract void RecordIntegerData<T>(string key, T data, TimeSpan timeout);

    protected abstract void RecordFloatingPointData<T>(string key, T data, TimeSpan timeout);

    internal RegisteredMetric GetMetric(string key)
    {
      if (this.IntegerMetrics.ContainsKey(key))
        return this.IntegerMetrics[key];
      return this.FloatingPointMetrics.ContainsKey(key) ? this.FloatingPointMetrics[key] : (RegisteredMetric) null;
    }

    internal void CloseMetric(string key)
    {
      if (this.IntegerMetrics.ContainsKey(key))
        this.IntegerMetrics.Remove(key);
      if (!this.FloatingPointMetrics.ContainsKey(key))
        return;
      this.FloatingPointMetrics.Remove(key);
    }

    private MetricDataType DetermineMetricDataType(Type specifiedType)
    {
      if (specifiedType == typeof (byte) || specifiedType == typeof (short) || specifiedType == typeof (int) || specifiedType == typeof (long))
        return MetricDataType.Integer;
      if (specifiedType == typeof (float) || specifiedType == typeof (double) || specifiedType == typeof (Decimal))
        return MetricDataType.FloatingPoint;
      throw new UnsupportedNumericStructException();
    }
  }
}
