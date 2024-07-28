// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.EventTelemetry
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
  public sealed class EventTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "Event";
    internal readonly string BaseType = typeof (EventData).Name;
    internal readonly EventData Data;
    private readonly TelemetryContext context;

    public EventTelemetry()
    {
      this.Data = new EventData();
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
    }

    public EventTelemetry(string name)
      : this()
    {
      this.Name = name;
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public string Name
    {
      get => this.Data.name;
      set => this.Data.name = value;
    }

    public int CommonSchemaVersion { get; set; } = 2;

    public IDictionary<string, double> Metrics => this.Data.measurements;

    public IDictionary<string, string> Properties => this.Data.properties;

    void ITelemetry.Sanitize()
    {
      this.Name = this.Name.SanitizeName();
      this.Name = Utils.PopulateRequiredStringValue(this.Name, "name", typeof (EventTelemetry).FullName);
      this.Properties.SanitizeProperties();
      this.Metrics.SanitizeMeasurements();
    }
  }
}
