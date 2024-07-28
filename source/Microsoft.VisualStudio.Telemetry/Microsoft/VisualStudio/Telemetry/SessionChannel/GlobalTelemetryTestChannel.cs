// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.GlobalTelemetryTestChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal sealed class GlobalTelemetryTestChannel : ISessionChannel
  {
    private static readonly GlobalTelemetryTestChannel PrivateInstance = new GlobalTelemetryTestChannel();

    public event EventHandler<TelemetryTestChannelEventArgs> EventPosted;

    public string ChannelId => "developerchannel";

    public bool IsStarted => true;

    public ChannelProperties Properties
    {
      get => ChannelProperties.DevChannel;
      set
      {
      }
    }

    public string TransportUsed => string.Empty;

    public static GlobalTelemetryTestChannel Instance => GlobalTelemetryTestChannel.PrivateInstance;

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      EventHandler<TelemetryTestChannelEventArgs> eventPosted = this.EventPosted;
      if (eventPosted == null)
        return;
      TelemetryTestChannelEventArgs e = new TelemetryTestChannelEventArgs()
      {
        Event = telemetryEvent
      };
      eventPosted((object) this, e);
    }

    public void ClearEventSubscribers() => this.EventPosted = (EventHandler<TelemetryTestChannelEventArgs>) null;

    public void PostEvent(
      TelemetryEvent telemetryEvent,
      IEnumerable<ITelemetryManifestRouteArgs> args)
    {
      this.PostEvent(telemetryEvent);
    }

    public void Start(string sessionId)
    {
    }

    private GlobalTelemetryTestChannel()
    {
    }

    public override string ToString() => this.ChannelId ?? "";
  }
}
