// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Notification.NotificationTelemetryChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry.Notification
{
  internal sealed class NotificationTelemetryChannel : ITelemetryTestChannel
  {
    private readonly Action<TelemetryEvent> handler;

    public NotificationTelemetryChannel(Action<TelemetryEvent> handler) => this.handler = handler;

    public void OnPostEvent(object sender, TelemetryTestChannelEventArgs e)
    {
      if (this.handler == null || e == null)
        return;
      this.handler(e.Event);
    }
  }
}
