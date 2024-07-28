// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.LbChannelState
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class LbChannelState : IDisposable
  {
    private readonly int maxRequestsPending;
    private readonly IChannel channel;
    private int requestsPending;
    private bool cachedHealthy = true;

    public LbChannelState(IChannel channel, int maxRequestsPending)
    {
      this.channel = channel;
      this.maxRequestsPending = maxRequestsPending;
    }

    public bool Enter()
    {
      if (Interlocked.Increment(ref this.requestsPending) <= this.maxRequestsPending)
        return true;
      Interlocked.Decrement(ref this.requestsPending);
      return false;
    }

    public bool Exit() => Interlocked.Decrement(ref this.requestsPending) == 0;

    public bool DeepHealthy
    {
      get
      {
        if (!this.ShallowHealthy)
          return false;
        int num = this.channel.Healthy ? 1 : 0;
        if (num != 0)
          return num != 0;
        this.cachedHealthy = false;
        Interlocked.MemoryBarrier();
        return num != 0;
      }
    }

    public bool ShallowHealthy
    {
      get
      {
        Interlocked.MemoryBarrier();
        return this.cachedHealthy;
      }
    }

    public IChannel Channel => this.channel;

    public void Dispose() => this.channel.Close();
  }
}
