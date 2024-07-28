// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.DefaultRemoteSettingsTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class DefaultRemoteSettingsTelemetry : IRemoteSettingsTelemetry
  {
    private TelemetrySession telemetrySession;

    public DefaultRemoteSettingsTelemetry(TelemetrySession telemetrySession)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.telemetrySession = telemetrySession;
    }

    public IRemoteSettingsTelemetryActivity CreateActivity(string name) => (IRemoteSettingsTelemetryActivity) new DefaultRemoteSettingsTelemetry.DefaultRemoteSettingsTelemetryActivity(this.telemetrySession, new TelemetryActivity(name));

    public void PostEvent(string name, IDictionary<string, object> properties)
    {
      TelemetryEvent telemetryEvent = new TelemetryEvent(name);
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
        telemetryEvent.Properties[property.Key] = property.Value;
      this.telemetrySession.PostEvent(telemetryEvent);
    }

    private class DefaultRemoteSettingsTelemetryActivity : IRemoteSettingsTelemetryActivity
    {
      private readonly TelemetryActivity telemetryActivity;
      private readonly TelemetrySession telemetrySession;

      public DefaultRemoteSettingsTelemetryActivity(
        TelemetrySession telemetrySession,
        TelemetryActivity telemetryActivity)
      {
        this.telemetryActivity = telemetryActivity;
        this.telemetrySession = telemetrySession;
      }

      public void End() => this.telemetryActivity.End();

      public void Post(IDictionary<string, object> properties)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
          this.telemetryActivity.Properties[property.Key] = property.Value;
        this.telemetrySession.PostEvent((TelemetryEvent) this.telemetryActivity);
      }

      public void Start() => this.telemetryActivity.Start();

      internal TelemetryActivity GetActivity() => this.telemetryActivity;
    }
  }
}
