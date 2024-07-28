// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelOpenArguments
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelOpenArguments
  {
    private readonly ChannelCommonArguments commonArguments;
    private readonly ChannelOpenTimeline openTimeline;
    private readonly int openTimeoutSeconds;
    private readonly PortReuseMode portReuseMode;
    private readonly UserPortPool userPortPool;
    private readonly RntbdConstants.CallerId callerId;

    public ChannelOpenArguments(
      Guid activityId,
      ChannelOpenTimeline openTimeline,
      int openTimeoutSeconds,
      PortReuseMode portReuseMode,
      UserPortPool userPortPool,
      RntbdConstants.CallerId callerId)
    {
      this.commonArguments = new ChannelCommonArguments(activityId, TransportErrorCode.ChannelOpenTimeout, false);
      this.openTimeline = openTimeline;
      this.openTimeoutSeconds = openTimeoutSeconds;
      this.portReuseMode = portReuseMode;
      this.userPortPool = userPortPool;
      this.callerId = callerId;
    }

    public ChannelCommonArguments CommonArguments => this.commonArguments;

    public ChannelOpenTimeline OpenTimeline => this.openTimeline;

    public int OpenTimeoutSeconds => this.openTimeoutSeconds;

    public PortReuseMode PortReuseMode => this.portReuseMode;

    public UserPortPool PortPool => this.userPortPool;

    public RntbdConstants.CallerId CallerId => this.callerId;
  }
}
