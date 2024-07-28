// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelProperties
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Net.Security;

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
      TimeSpan localRegionOpenTimeout,
      PortReuseMode portReuseMode,
      UserPortPool userPortPool,
      int maxChannels,
      int partitionCount,
      int maxRequestsPerChannel,
      int maxConcurrentOpeningConnectionCount,
      TimeSpan receiveHangDetectionTime,
      TimeSpan sendHangDetectionTime,
      TimeSpan idleTimeout,
      TimerPool idleTimerPool,
      RntbdConstants.CallerId callerId,
      bool enableChannelMultiplexing,
      MemoryStreamPool memoryStreamPool,
      RemoteCertificateValidationCallback remoteCertificateValidationCallback = null)
    {
      this.UserAgent = userAgent;
      this.CertificateHostNameOverride = certificateHostNameOverride;
      this.ConnectionStateListener = connectionStateListener;
      this.RequestTimerPool = requestTimerPool;
      this.RequestTimeout = requestTimeout;
      this.OpenTimeout = openTimeout;
      this.LocalRegionOpenTimeout = localRegionOpenTimeout;
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
      this.EnableChannelMultiplexing = enableChannelMultiplexing;
      this.MaxConcurrentOpeningConnectionCount = maxConcurrentOpeningConnectionCount;
      this.MemoryStreamPool = memoryStreamPool;
      this.RemoteCertificateValidationCallback = remoteCertificateValidationCallback;
    }

    public UserAgentContainer UserAgent { get; private set; }

    public string CertificateHostNameOverride { get; private set; }

    public IConnectionStateListener ConnectionStateListener { get; private set; }

    public TimerPool RequestTimerPool { get; private set; }

    public TimerPool IdleTimerPool { get; private set; }

    public TimeSpan RequestTimeout { get; private set; }

    public TimeSpan OpenTimeout { get; private set; }

    public TimeSpan LocalRegionOpenTimeout { get; private set; }

    public PortReuseMode PortReuseMode { get; private set; }

    public int MaxChannels { get; private set; }

    public int PartitionCount { get; private set; }

    public int MaxRequestsPerChannel { get; private set; }

    public TimeSpan ReceiveHangDetectionTime { get; private set; }

    public TimeSpan SendHangDetectionTime { get; private set; }

    public TimeSpan IdleTimeout { get; private set; }

    public UserPortPool UserPortPool { get; private set; }

    public RntbdConstants.CallerId CallerId { get; private set; }

    public bool EnableChannelMultiplexing { get; private set; }

    public int MaxConcurrentOpeningConnectionCount { get; private set; }

    public MemoryStreamPool MemoryStreamPool { get; private set; }

    public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; private set; }
  }
}
