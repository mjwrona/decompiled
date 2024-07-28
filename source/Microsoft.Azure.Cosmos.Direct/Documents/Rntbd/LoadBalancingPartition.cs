// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.LoadBalancingPartition
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
    private readonly bool localRegionRequest;
    private readonly int maxCapacity;
    private int requestsPending;
    private readonly LoadBalancingPartition.SequenceGenerator sequenceGenerator = new LoadBalancingPartition.SequenceGenerator();
    private readonly ReaderWriterLockSlim capacityLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private int capacity;
    private readonly List<LbChannelState> openChannels = new List<LbChannelState>();
    private readonly SemaphoreSlim concurrentOpeningChannelSlim;

    public LoadBalancingPartition(
      Uri serverUri,
      ChannelProperties channelProperties,
      bool localRegionRequest)
    {
      this.serverUri = serverUri;
      this.channelProperties = channelProperties;
      this.localRegionRequest = localRegionRequest;
      this.maxCapacity = checked (channelProperties.MaxChannels * channelProperties.MaxRequestsPerChannel);
      this.concurrentOpeningChannelSlim = new SemaphoreSlim(channelProperties.MaxConcurrentOpeningConnectionCount, channelProperties.MaxConcurrentOpeningConnectionCount);
    }

    public async Task<StoreResponse> RequestAsync(
      DocumentServiceRequest request,
      TransportAddressUri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId,
      TransportRequestStats transportRequestStats)
    {
      int currentPending = Interlocked.Increment(ref this.requestsPending);
      transportRequestStats.NumberOfInflightRequestsToEndpoint = new int?(currentPending);
      StoreResponse storeResponse;
      try
      {
        if (currentPending > this.maxCapacity)
          throw new RequestRateTooLargeException(string.Format("All connections to {0} are fully utilized. Increase the maximum number of connections or the maximum number of requests per connection", (object) this.serverUri), SubStatusCodes.ClientTcpChannelFull);
        transportRequestStats.RecordState(TransportRequestStats.RequestStage.ChannelAcquisitionStarted);
        while (true)
        {
          LbChannelState channelState = (LbChannelState) null;
          bool flag = false;
          uint num1 = this.sequenceGenerator.Next();
          this.capacityLock.EnterReadLock();
          try
          {
            transportRequestStats.NumberOfOpenConnectionsToEndpoint = new int?(this.openChannels.Count);
            if (currentPending <= this.capacity)
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
                storeResponse = await channelState.Channel.RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
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
            int targetChannels = MathUtils.CeilingMultiple(currentPending, this.channelProperties.MaxRequestsPerChannel) / this.channelProperties.MaxRequestsPerChannel;
            int channelsCreated = 0;
            this.capacityLock.EnterWriteLock();
            try
            {
              if (this.openChannels.Count < targetChannels)
                channelsCreated = targetChannels - this.openChannels.Count;
              while (this.openChannels.Count < targetChannels)
                await this.OpenChannelAndIncrementCapacity(activityId, false);
            }
            finally
            {
              this.capacityLock.ExitWriteLock();
            }
            if (channelsCreated > 0)
              DefaultTrace.TraceInformation("Opened {0} channels to server {1}", (object) channelsCreated, (object) this.serverUri);
          }
          channelState = (LbChannelState) null;
        }
      }
      finally
      {
        currentPending = Interlocked.Decrement(ref this.requestsPending);
      }
      return storeResponse;
    }

    internal Task OpenChannelAsync(Guid activityId)
    {
      this.capacityLock.EnterWriteLock();
      try
      {
        return this.OpenChannelAndIncrementCapacity(activityId, true);
      }
      finally
      {
        this.capacityLock.ExitWriteLock();
      }
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

    private async Task OpenChannelAndIncrementCapacity(
      Guid activityId,
      bool waitForBackgroundInitializationComplete)
    {
      Channel newChannel = new Channel(activityId, this.serverUri, this.channelProperties, this.localRegionRequest, this.concurrentOpeningChannelSlim);
      if (waitForBackgroundInitializationComplete)
        await newChannel.OpenChannelAsync(activityId);
      this.openChannels.Add(new LbChannelState((IChannel) newChannel, this.channelProperties.MaxRequestsPerChannel));
      this.capacity += this.channelProperties.MaxRequestsPerChannel;
      newChannel = (Channel) null;
    }

    private sealed class SequenceGenerator
    {
      private int current;

      public uint Next() => (uint) (2147483648UL + (ulong) Interlocked.Increment(ref this.current));
    }
  }
}
