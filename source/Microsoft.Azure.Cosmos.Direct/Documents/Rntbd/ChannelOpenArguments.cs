// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelOpenArguments
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelOpenArguments
  {
    private readonly ChannelCommonArguments commonArguments;
    private readonly ChannelOpenTimeline openTimeline;
    private readonly TimeSpan openTimeout;
    private readonly PortReuseMode portReuseMode;
    private readonly UserPortPool userPortPool;
    private readonly RntbdConstants.CallerId callerId;

    public ChannelOpenArguments(
      Guid activityId,
      ChannelOpenTimeline openTimeline,
      TimeSpan openTimeout,
      PortReuseMode portReuseMode,
      UserPortPool userPortPool,
      RntbdConstants.CallerId callerId)
    {
      this.commonArguments = new ChannelCommonArguments(activityId, TransportErrorCode.ChannelOpenTimeout, false);
      this.openTimeline = openTimeline;
      this.openTimeout = openTimeout;
      this.portReuseMode = portReuseMode;
      this.userPortPool = userPortPool;
      this.callerId = callerId;
    }

    public ChannelCommonArguments CommonArguments => this.commonArguments;

    public ChannelOpenTimeline OpenTimeline => this.openTimeline;

    public TimeSpan OpenTimeout => this.openTimeout;

    public PortReuseMode PortReuseMode => this.portReuseMode;

    public UserPortPool PortPool => this.userPortPool;

    public RntbdConstants.CallerId CallerId => this.callerId;
  }
}
