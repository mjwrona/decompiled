// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelProperties
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelProperties
  {
    public ChannelProperties(
      UserAgentContainer userAgent,
      string certificateHostNameOverride,
      IConnectionStateListener connectionStateListener,
      TimerPool requestTimerPool,
      TimeSpan requestTimeout,
      TimeSpan openTimeout,
      PortReuseMode portReuseMode,
      UserPortPool userPortPool,
      int maxChannels,
      int partitionCount,
      int maxRequestsPerChannel,
      TimeSpan receiveHangDetectionTime,
      TimeSpan sendHangDetectionTime,
      TimeSpan idleTimeout,
      TimerPool idleTimerPool,
      RntbdConstants.CallerId callerId)
    {
      this.UserAgent = userAgent;
      this.CertificateHostNameOverride = certificateHostNameOverride;
      this.ConnectionStateListener = connectionStateListener;
      this.RequestTimerPool = requestTimerPool;
      this.RequestTimeout = requestTimeout;
      this.OpenTimeout = openTimeout;
      this.PortReuseMode = portReuseMode;
      this.UserPortPool = userPortPool;
      this.MaxChannels = maxChannels;
      this.PartitionCount = partitionCount;
      this.MaxRequestsPerChannel = maxRequestsPerChannel;
      this.ReceiveHangDetectionTime = receiveHangDetectionTime;
      this.SendHangDetectionTime = sendHangDetectionTime;
      this.IdleTimeout = idleTimeout;
      this.IdleTimerPool = idleTimerPool;
      this.CallerId = callerId;
    }

    public UserAgentContainer UserAgent { get; private set; }

    public string CertificateHostNameOverride { get; private set; }

    public IConnectionStateListener ConnectionStateListener { get; private set; }

    public TimerPool RequestTimerPool { get; private set; }

    public TimerPool IdleTimerPool { get; private set; }

    public TimeSpan RequestTimeout { get; private set; }

    public TimeSpan OpenTimeout { get; private set; }

    public PortReuseMode PortReuseMode { get; private set; }

    public int MaxChannels { get; private set; }

    public int PartitionCount { get; private set; }

    public int MaxRequestsPerChannel { get; private set; }

    public TimeSpan ReceiveHangDetectionTime { get; private set; }

    public TimeSpan SendHangDetectionTime { get; private set; }

    public TimeSpan IdleTimeout { get; private set; }

    public UserPortPool UserPortPool { get; private set; }

    public RntbdConstants.CallerId CallerId { get; private set; }
  }
}
