// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.TelemetryBufferChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal class TelemetryBufferChannel : ISessionChannel
  {
    private readonly ConcurrentQueue<TelemetryEvent> eventBuffer = new ConcurrentQueue<TelemetryEvent>();

    public string ChannelId => "bufferChannel";

    public string TransportUsed => this.ChannelId;

    public ChannelProperties Properties
    {
      get => ChannelProperties.None;
      set => throw new MemberAccessException("it is not allowed to change properties for this channel");
    }

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      this.eventBuffer.Enqueue(telemetryEvent);
    }

    public void PostEvent(
      TelemetryEvent telemetryEvent,
      IEnumerable<ITelemetryManifestRouteArgs> args)
    {
      this.PostEvent(telemetryEvent);
    }

    public void Start(string sessionID)
    {
    }

    public bool IsStarted => true;

    public bool TryDequeue(out TelemetryEvent telemetryEvent) => this.eventBuffer.TryDequeue(out telemetryEvent);

    public override string ToString() => string.Format("{0} Cnt = {1}", (object) this.ChannelId, (object) this.eventBuffer.Count);
  }
}
