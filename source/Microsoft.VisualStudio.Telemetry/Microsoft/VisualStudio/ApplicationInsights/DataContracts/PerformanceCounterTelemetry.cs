// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.PerformanceCounterTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  [DebuggerDisplay("CategoryName={CategoryName}; CounterName={CounterName}; InstanceName={InstanceName}; Value={Value}; Timestamp={Timestamp}")]
  internal class PerformanceCounterTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "PerformanceCounter";
    internal readonly string BaseType = typeof (PerformanceCounterData).Name;
    internal readonly PerformanceCounterData Data;
    private TelemetryContext context;

    public PerformanceCounterTelemetry() => this.Data = new PerformanceCounterData();

    public PerformanceCounterTelemetry(
      string categoryName,
      string counterName,
      string instanceName,
      double value)
      : this()
    {
      this.CategoryName = categoryName;
      this.CounterName = counterName;
      this.InstanceName = instanceName;
      this.Value = value;
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => LazyInitializer.EnsureInitialized<TelemetryContext>(ref this.context);

    public double Value
    {
      get => this.Data.value;
      set => this.Data.value = value;
    }

    public string CategoryName
    {
      get => this.Data.categoryName;
      set => this.Data.categoryName = value;
    }

    public string CounterName
    {
      get => this.Data.counterName;
      set => this.Data.counterName = value;
    }

    public string InstanceName
    {
      get => this.Data.instanceName;
      set => this.Data.instanceName = value;
    }

    public IDictionary<string, string> Properties => this.Data.properties;

    void ITelemetry.Sanitize()
    {
    }
  }
}
