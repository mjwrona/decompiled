// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Notification.TelemetryNotificationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry.Notification
{
  internal class TelemetryNotificationProvider : ITelemetryNotificationProvider
  {
    private ITelemetryTestChannel channel;
    private TelemetrySession telemetrySession;

    public TelemetryNotificationProvider(TelemetrySession session) => this.telemetrySession = session;

    public void AttachChannel(ITelemetryTestChannel channel)
    {
      this.channel = channel;
      if (this.telemetrySession == null)
        this.telemetrySession = TelemetryService.DefaultSession;
      this.telemetrySession.RawTelemetryEventReceived += new EventHandler<TelemetryTestChannelEventArgs>(channel.OnPostEvent);
    }

    public void DetachChannel(ITelemetryTestChannel channel)
    {
      if (this.telemetrySession != null)
        this.telemetrySession.RawTelemetryEventReceived -= new EventHandler<TelemetryTestChannelEventArgs>(channel.OnPostEvent);
      this.channel = (ITelemetryTestChannel) null;
    }

    public void PostFaultEvent(string eventName, string description, Exception exception) => TelemetryService.DefaultSession.PostFault(eventName, description, exception);
  }
}
