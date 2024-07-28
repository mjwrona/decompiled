// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.DefaultExperimentationTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class DefaultExperimentationTelemetry : 
    IExperimentationTelemetry3,
    IExperimentationTelemetry2,
    IExperimentationTelemetry
  {
    private readonly TelemetrySession telemetrySession;

    public DefaultExperimentationTelemetry(TelemetrySession telemetrySession)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.telemetrySession = telemetrySession;
    }

    public void PostEvent(string name, IDictionary<string, string> properties)
    {
      TelemetryEvent telemetryEvent = new TelemetryEvent(name);
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) properties)
        telemetryEvent.Properties[property.Key] = (object) property.Value;
      this.telemetrySession.PostEvent(telemetryEvent);
    }

    public void SetSharedProperty(string name, string value) => this.telemetrySession.SetSharedProperty(name, (object) value);

    public void PostFault(string eventName, string description) => this.telemetrySession.PostFault(eventName, description);

    public void PostEvent(string name, IDictionary<string, object> properties)
    {
      TelemetryEvent telemetryEvent = new TelemetryEvent(name);
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
        telemetryEvent.Properties[property.Key] = property.Value;
      this.telemetrySession.PostEvent(telemetryEvent);
    }

    public void SetSharedProperty(string name, object value) => this.telemetrySession.SetSharedProperty(name, value);
  }
}
