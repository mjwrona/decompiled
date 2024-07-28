// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelOpenTimeline
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelOpenTimeline
  {
    private readonly DateTimeOffset creationTime;
    private DateTimeOffset connectTime = DateTimeOffset.MinValue;
    private DateTimeOffset sslHandshakeTime = DateTimeOffset.MinValue;
    private DateTimeOffset rntbdHandshakeTime = DateTimeOffset.MinValue;

    public ChannelOpenTimeline() => this.creationTime = DateTimeOffset.UtcNow;

    public void RecordConnectFinishTime() => this.connectTime = DateTimeOffset.UtcNow;

    public void RecordSslHandshakeFinishTime() => this.sslHandshakeTime = DateTimeOffset.UtcNow;

    public void RecordRntbdHandshakeFinishTime() => this.rntbdHandshakeTime = DateTimeOffset.UtcNow;

    public void WriteTrace()
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      ChannelOpenTimeline.ConnectionTimerDelegate traceFunc = ChannelOpenTimeline.TraceFunc;
      if (traceFunc == null)
        return;
      traceFunc(Trace.CorrelationManager.ActivityId, ChannelOpenTimeline.InvariantString(this.creationTime), ChannelOpenTimeline.InvariantString(this.connectTime), ChannelOpenTimeline.InvariantString(this.sslHandshakeTime), ChannelOpenTimeline.InvariantString(this.rntbdHandshakeTime), ChannelOpenTimeline.InvariantString(utcNow));
    }

    public static void LegacyWriteTrace(RntbdConnectionOpenTimers timers)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      ChannelOpenTimeline.ConnectionTimerDelegate traceFunc = ChannelOpenTimeline.TraceFunc;
      if (traceFunc == null)
        return;
      traceFunc(Trace.CorrelationManager.ActivityId, ChannelOpenTimeline.InvariantString(timers.CreationTimestamp), ChannelOpenTimeline.InvariantString(timers.TcpConnectCompleteTimestamp), ChannelOpenTimeline.InvariantString(timers.SslHandshakeCompleteTimestamp), ChannelOpenTimeline.InvariantString(timers.RntbdHandshakeCompleteTimestamp), ChannelOpenTimeline.InvariantString(utcNow));
    }

    public static ChannelOpenTimeline.ConnectionTimerDelegate TraceFunc { get; set; }

    private static string InvariantString(DateTimeOffset t) => t.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);

    public delegate void ConnectionTimerDelegate(
      Guid activityId,
      string connectionCreationTime,
      string tcpConnectCompleteTime,
      string sslHandshakeCompleteTime,
      string rntbdHandshakeCompleteTime,
      string openTaskCompletionTime);
  }
}
