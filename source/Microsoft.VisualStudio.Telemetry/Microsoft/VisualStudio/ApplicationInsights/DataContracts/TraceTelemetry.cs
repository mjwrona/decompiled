// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.TraceTelemetry
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
  public sealed class TraceTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "Message";
    internal readonly string BaseType = typeof (MessageData).Name;
    internal readonly MessageData Data;
    private readonly TelemetryContext context;

    public TraceTelemetry()
    {
      this.Data = new MessageData();
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
    }

    public TraceTelemetry(string message)
      : this()
    {
      this.Message = message;
    }

    public TraceTelemetry(string message, Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel severityLevel)
      : this(message)
    {
      this.SeverityLevel = new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?(severityLevel);
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public string Message
    {
      get => this.Data.message;
      set => this.Data.message = value;
    }

    public Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel? SeverityLevel
    {
      get => this.Data.severityLevel.TranslateSeverityLevel();
      set => this.Data.severityLevel = value.TranslateSeverityLevel();
    }

    public IDictionary<string, string> Properties => this.Data.properties;

    void ITelemetry.Sanitize()
    {
      this.Data.message = this.Data.message.SanitizeMessage();
      this.Data.message = Utils.PopulateRequiredStringValue(this.Data.message, "message", typeof (TraceTelemetry).FullName);
      this.Data.properties.SanitizeProperties();
    }
  }
}
