// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.MetricTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  public sealed class MetricTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "Metric";
    internal readonly string BaseType = typeof (MetricData).Name;
    internal readonly MetricData Data;
    internal readonly DataPoint Metric;
    private readonly TelemetryContext context;
    private bool isAggregation;

    public MetricTelemetry()
    {
      this.Data = new MetricData();
      this.Metric = new DataPoint();
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
      this.Data.metrics.Add(this.Metric);
    }

    public MetricTelemetry(string metricName, double metricValue)
      : this()
    {
      this.Name = metricName;
      this.Value = metricValue;
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public string Name
    {
      get => this.Metric.name;
      set => this.Metric.name = value;
    }

    public double Value
    {
      get => this.Metric.value;
      set => this.Metric.value = value;
    }

    public int? Count
    {
      get => this.Metric.count;
      set
      {
        this.Metric.count = value;
        this.UpdateKind();
      }
    }

    public double? Min
    {
      get => this.Metric.min;
      set
      {
        this.Metric.min = value;
        this.UpdateKind();
      }
    }

    public double? Max
    {
      get => this.Metric.max;
      set
      {
        this.Metric.max = value;
        this.UpdateKind();
      }
    }

    public double? StandardDeviation
    {
      get => this.Metric.stdDev;
      set
      {
        this.Metric.stdDev = value;
        this.UpdateKind();
      }
    }

    public IDictionary<string, string> Properties => this.Data.properties;

    void ITelemetry.Sanitize()
    {
      this.Name = this.Name.SanitizeName();
      this.Name = Utils.PopulateRequiredStringValue(this.Name, "name", typeof (MetricTelemetry).FullName);
      this.Properties.SanitizeProperties();
    }

    private void UpdateKind()
    {
      bool flag = this.Metric.count.HasValue || this.Metric.min.HasValue || this.Metric.max.HasValue || this.Metric.stdDev.HasValue;
      if (this.isAggregation != flag)
        this.Metric.kind = !flag ? DataPointType.Measurement : DataPointType.Aggregation;
      this.isAggregation = flag;
    }
  }
}
