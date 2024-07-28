// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelDictionary
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelDictionary : IDisposable
  {
    private readonly ChannelProperties channelProperties;
    private bool disposed;
    private ConcurrentDictionary<ServerKey, IChannel> channels = new ConcurrentDictionary<ServerKey, IChannel>();

    public ChannelDictionary(ChannelProperties channelProperties) => this.channelProperties = channelProperties;

    public IChannel GetChannel(Uri requestUri, bool localRegionRequest)
    {
      this.ThrowIfDisposed();
      ServerKey key = new ServerKey(requestUri);
      IChannel channel1 = (IChannel) null;
      if (this.channels.TryGetValue(key, out channel1))
        return channel1;
      IChannel channel2 = (IChannel) new LoadBalancingChannel(new Uri(requestUri.GetLeftPart(UriPartial.Authority)), this.channelProperties, localRegionRequest);
      if (this.channels.TryAdd(key, channel2))
        return channel2;
      this.channels.TryGetValue(key, out channel2);
      return channel2;
    }

    public Task OpenChannelAsync(Uri physicalAddress, bool localRegionRequest, Guid activityId)
    {
      if (this.channels.ContainsKey(new ServerKey(physicalAddress)))
        return (Task) Task.FromResult<int>(0);
      this.ThrowIfDisposed();
      return this.GetChannel(physicalAddress, localRegionRequest).OpenChannelAsync(activityId);
    }

    public void Dispose()
    {
      this.ThrowIfDisposed();
      this.disposed = true;
      foreach (IChannel channel in (IEnumerable<IChannel>) this.channels.Values)
        channel.Close();
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (ChannelDictionary));
    }
  }
}
