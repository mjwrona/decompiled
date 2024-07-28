// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelDictionary
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelDictionary : IDisposable
  {
    private readonly ChannelProperties channelProperties;
    private bool disposed;
    private ConcurrentDictionary<ServerKey, IChannel> channels = new ConcurrentDictionary<ServerKey, IChannel>();

    public ChannelDictionary(ChannelProperties channelProperties) => this.channelProperties = channelProperties;

    public IChannel GetChannel(Uri requestUri)
    {
      this.ThrowIfDisposed();
      ServerKey key = new ServerKey(requestUri);
      IChannel channel1 = (IChannel) null;
      if (this.channels.TryGetValue(key, out channel1))
        return channel1;
      IChannel channel2 = (IChannel) new LoadBalancingChannel(new Uri(requestUri.GetLeftPart(UriPartial.Authority)), this.channelProperties);
      if (this.channels.TryAdd(key, channel2))
        return channel2;
      this.channels.TryGetValue(key, out channel2);
      return channel2;
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
