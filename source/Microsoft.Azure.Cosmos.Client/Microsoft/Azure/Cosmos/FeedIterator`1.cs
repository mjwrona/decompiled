// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIterator`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class FeedIterator<T> : IDisposable
  {
    private bool disposedValue;
    internal ContainerInternal container;
    internal string databaseName;

    public abstract bool HasMoreResults { get; }

    public abstract Task<FeedResponse<T>> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken));

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      this.disposedValue = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
