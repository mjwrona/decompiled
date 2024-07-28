// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.SessionStateTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  public sealed class SessionStateTelemetry : ITelemetry
  {
    internal const string TelemetryName = "SessionState";
    internal readonly SessionStateData Data;
    private readonly TelemetryContext context;

    public SessionStateTelemetry()
    {
      this.Data = new SessionStateData();
      this.context = new TelemetryContext((IDictionary<string, string>) new Dictionary<string, string>(), (IDictionary<string, string>) new Dictionary<string, string>());
    }

    public SessionStateTelemetry(SessionState state)
      : this()
    {
      this.State = state;
    }

    public DateTimeOffset Timestamp { get; set; }

    public TelemetryContext Context => this.context;

    public string Sequence { get; set; }

    public SessionState State { get; set; }

    void ITelemetry.Sanitize()
    {
    }
  }
}
