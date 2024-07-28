// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.DefaultTargetedNotificationsTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class DefaultTargetedNotificationsTelemetry : ITargetedNotificationsTelemetry
  {
    private TelemetrySession telemetrySession;

    public DefaultTargetedNotificationsTelemetry(TelemetrySession telemetrySession)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.telemetrySession = telemetrySession;
    }

    public string SessionId => this.telemetrySession.SessionId;

    public void PostCriticalFault(
      string eventName,
      string description,
      Exception exception = null,
      Dictionary<string, object> additionalProperties = null)
    {
      this.PostEventInternal((TelemetryEvent) new FaultEvent(eventName, description, FaultSeverity.Critical, exception), additionalProperties);
    }

    public void PostDiagnosticFault(
      string eventName,
      string description,
      Exception exception = null,
      Dictionary<string, object> additionalProperties = null)
    {
      this.PostEventInternal((TelemetryEvent) new FaultEvent(eventName, description, FaultSeverity.Diagnostic, exception), additionalProperties);
    }

    public void PostGeneralFault(
      string eventName,
      string description,
      Exception exception = null,
      Dictionary<string, object> additionalProperties = null)
    {
      this.PostEventInternal((TelemetryEvent) new FaultEvent(eventName, description, FaultSeverity.General, exception), additionalProperties);
    }

    public void PostSuccessfulOperation(
      string eventName,
      Dictionary<string, object> additionalProperties = null)
    {
      this.PostEventInternal((TelemetryEvent) new OperationEvent(eventName, TelemetryResult.Success), additionalProperties);
    }

    private void PostEventInternal(
      TelemetryEvent telemetryEvent,
      Dictionary<string, object> additionalProperties = null)
    {
      if (additionalProperties != null)
      {
        foreach (string key in additionalProperties.Keys)
          telemetryEvent.Properties[key] = additionalProperties[key];
      }
      this.telemetrySession.PostEvent(telemetryEvent);
    }
  }
}
