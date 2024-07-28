// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.LoadBalancingChannel
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class LoadBalancingChannel : IChannel, IDisposable
  {
    private readonly Uri serverUri;
    private readonly LoadBalancingPartition singlePartition;
    private readonly LoadBalancingPartition[] partitions;
    private bool disposed;

    public LoadBalancingChannel(
      Uri serverUri,
      ChannelProperties channelProperties,
      bool localRegionRequest)
    {
      this.serverUri = serverUri;
      if (channelProperties.PartitionCount < 1 || channelProperties.PartitionCount > 8)
        throw new ArgumentOutOfRangeException("PartitionCount", (object) channelProperties.PartitionCount, "The partition count must be between 1 and 8");
      if (channelProperties.PartitionCount > 1)
      {
        ChannelProperties channelProperties1 = new ChannelProperties(channelProperties.UserAgent, channelProperties.CertificateHostNameOverride, channelProperties.ConnectionStateListener, channelProperties.RequestTimerPool, channelProperties.RequestTimeout, channelProperties.OpenTimeout, channelProperties.LocalRegionOpenTimeout, channelProperties.PortReuseMode, channelProperties.UserPortPool, MathUtils.CeilingMultiple(channelProperties.MaxChannels, channelProperties.PartitionCount) / channelProperties.PartitionCount, 1, channelProperties.MaxRequestsPerChannel, channelProperties.MaxConcurrentOpeningConnectionCount, channelProperties.ReceiveHangDetectionTime, channelProperties.SendHangDetectionTime, channelProperties.IdleTimeout, channelProperties.IdleTimerPool, channelProperties.CallerId, channelProperties.EnableChannelMultiplexing, channelProperties.MemoryStreamPool);
        this.partitions = new LoadBalancingPartition[channelProperties.PartitionCount];
        for (int index = 0; index < this.partitions.Length; ++index)
          this.partitions[index] = new LoadBalancingPartition(serverUri, channelProperties1, localRegionRequest);
      }
      else
        this.singlePartition = new LoadBalancingPartition(serverUri, channelProperties, localRegionRequest);
    }

    public bool Healthy
    {
      get
      {
        this.ThrowIfDisposed();
        return true;
      }
    }

    public Task<StoreResponse> RequestAsync(
      DocumentServiceRequest request,
      TransportAddressUri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId,
      TransportRequestStats transportRequestStats)
    {
      this.ThrowIfDisposed();
      return this.singlePartition != null ? this.singlePartition.RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats) : this.GetLoadBalancedPartition(activityId).RequestAsync(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
    }

    public Task OpenChannelAsync(Guid activityId)
    {
      this.ThrowIfDisposed();
      return this.singlePartition != null ? this.singlePartition.OpenChannelAsync(activityId) : this.GetLoadBalancedPartition(activityId).OpenChannelAsync(activityId);
    }

    private LoadBalancingPartition GetLoadBalancedPartition(Guid activityId) => this.partitions[((long) activityId.GetHashCode() & 2415919103L) % (long) this.partitions.Length];

    public void Close() => ((IDisposable) this).Dispose();

    void IDisposable.Dispose()
    {
      this.ThrowIfDisposed();
      this.disposed = true;
      if (this.singlePartition != null)
        this.singlePartition.Dispose();
      if (this.partitions == null)
        return;
      for (int index = 0; index < this.partitions.Length; ++index)
        this.partitions[index].Dispose();
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(string.Format("{0}:{1}", (object) nameof (LoadBalancingChannel), (object) this.serverUri));
    }
  }
}
