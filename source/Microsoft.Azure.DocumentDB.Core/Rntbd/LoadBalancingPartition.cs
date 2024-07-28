// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.LoadBalancingPartition
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class LoadBalancingPartition : IDisposable
  {
    private readonly Uri serverUri;
    private readonly ChannelProperties channelProperties;
    private readonly int maxCapacity;
    private int requestsPending;
    private readonly LoadBalancingPartition.SequenceGenerator sequenceGenerator = new LoadBalancingPartition.SequenceGenerator();
    private readonly ReaderWriterLockSlim capacityLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private int capacity;
    private readonly List<LbChannelState> openChannels = new List<LbChannelState>();

    public LoadBalancingPartition(Uri serverUri, ChannelProperties channelProperties)
    {
      this.serverUri = serverUri;
      this.channelProperties = channelProperties;
      this.maxCapacity = checked (channelProperties.MaxChannels * channelProperties.MaxRequestsPerChannel);
    }

    public async Task<StoreResponse> RequestAsync(
      DocumentServiceRequest request,
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId)
    {
      int x = Interlocked.Increment(ref this.requestsPending);
      StoreResponse storeResponse;
      try
      {
        if (x > this.maxCapacity)
          throw new RequestRateTooLargeException(string.Format("All connections to {0} are fully utilized. Increase the maximum number of connections or the maximum number of requests per connection", (object) this.serverUri), SubStatusCodes.ClientTcpChannelFull);
        while (true)
        {
          LbChannelState channelState = (LbChannelState) null;
          bool flag = false;
          uint num1 = this.sequenceGenerator.Next();
          this.capacityLock.EnterReadLock();
          try
          {
            if (x <= this.capacity)
            {
              LbChannelState openChannel = this.openChannels[(int) ((long) num1 % (long) this.openChannels.Count)];
              if (openChannel.Enter())
                channelState = openChannel;
            }
            else
              flag = true;
          }
          finally
          {
            this.capacityLock.ExitReadLock();
          }
          if (channelState != null)
          {
            try
            {
              if (channelState.DeepHealthy)
              {
                storeResponse = await channelState.Channel.RequestAsync(request, physicalAddress, resourceOperation, activityId);
                break;
              }
              this.capacityLock.EnterWriteLock();
              try
              {
                if (this.openChannels.Remove(channelState))
                  this.capacity -= this.channelProperties.MaxRequestsPerChannel;
              }
              finally
              {
                this.capacityLock.ExitWriteLock();
              }
            }
            finally
            {
              int num2;
              if (num2 < 0 && channelState.Exit() && !channelState.ShallowHealthy)
              {
                channelState.Dispose();
                DefaultTrace.TraceInformation("Closed unhealthy channel {0}", (object) channelState.Channel);
              }
            }
          }
          else if (flag)
          {
            int num3 = MathUtils.CeilingMultiple(x, this.channelProperties.MaxRequestsPerChannel) / this.channelProperties.MaxRequestsPerChannel;
            int num4 = 0;
            this.capacityLock.EnterWriteLock();
            try
            {
              if (this.openChannels.Count < num3)
                num4 = num3 - this.openChannels.Count;
              while (this.openChannels.Count < num3)
              {
                Channel channel = new Channel(activityId, this.serverUri, this.channelProperties);
                channel.Initialize();
                this.openChannels.Add(new LbChannelState((IChannel) channel, this.channelProperties.MaxRequestsPerChannel));
                this.capacity += this.channelProperties.MaxRequestsPerChannel;
              }
            }
            finally
            {
              this.capacityLock.ExitWriteLock();
            }
            if (num4 > 0)
              DefaultTrace.TraceInformation("Opened {0} channels to server {1}", (object) num4, (object) this.serverUri);
          }
          channelState = (LbChannelState) null;
        }
      }
      finally
      {
        Interlocked.Decrement(ref this.requestsPending);
      }
      return storeResponse;
    }

    public void Dispose()
    {
      this.capacityLock.EnterWriteLock();
      try
      {
        foreach (LbChannelState openChannel in this.openChannels)
          openChannel.Dispose();
      }
      finally
      {
        this.capacityLock.ExitWriteLock();
      }
      this.capacityLock.Dispose();
    }

    private sealed class SequenceGenerator
    {
      private int current;

      public uint Next() => (uint) (2147483648UL + (ulong) Interlocked.Increment(ref this.current));
    }
  }
}
