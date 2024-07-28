// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.RemoteDependencyTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  [DebuggerDisplay("Value={Value}; Name={Name}; Count={Count}; Success={Success}; Async={Async}; Timestamp={Timestamp}")]
  internal sealed class RemoteDependencyTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "RemoteDependency";
    internal readonly string BaseType = typeof (RemoteDependencyData).Name;
    internal readonly RemoteDependencyData Data;
    private readonly TelemetryContext context;

    public RemoteDependencyTelemetry()
    {
      this.Data = new RemoteDependencyData()
      {
        kind = DataPointType.Aggregation
      };
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public string Name
    {
      get => this.Data.name;
      set => this.Data.name = value;
    }

    public string CommandName
    {
      get => this.Data.commandName;
      set => this.Data.commandName = value;
    }

    public DependencyKind DependencyKind
    {
      get => this.Data.dependencyKind;
      set => this.Data.dependencyKind = value;
    }

    public double Value
    {
      get => this.Data.value;
      set => this.Data.value = value;
    }

    public int? Count
    {
      get => this.Data.count;
      set => this.Data.count = value;
    }

    public bool? Success
    {
      get => this.Data.success;
      set => this.Data.success = value;
    }

    public bool? Async
    {
      get => this.Data.async;
      set => this.Data.async = value;
    }

    public DependencySourceType DependencySource
    {
      get => this.Data.dependencySource;
      set => this.Data.dependencySource = value;
    }

    public IDictionary<string, string> Properties => this.Data.properties;

    void ITelemetry.Sanitize()
    {
      this.Name = this.Name.SanitizeName();
      this.Properties.SanitizeProperties();
    }
  }
}
