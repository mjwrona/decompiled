// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConnectionPool
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ConnectionPool : IDisposable
  {
    private const int MaxPoolFailureRetries = 3;
    private readonly string address;
    private readonly IConnectionDispenser connectionDispenser;
    private ConcurrentStack<IConnection> connections;
    private int ConcurrentConnectionMaxOpen;
    private SemaphoreSlim semaphore;
    private bool isDisposed;

    public ConnectionPool(
      string address,
      IConnectionDispenser connectionDispenser,
      int maxConcurrentConnectionOpenRequests)
    {
      this.address = address;
      this.connectionDispenser = connectionDispenser;
      this.connections = new ConcurrentStack<IConnection>();
      this.ConcurrentConnectionMaxOpen = maxConcurrentConnectionOpenRequests;
      this.semaphore = new SemaphoreSlim(maxConcurrentConnectionOpenRequests);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
      {
        this.DisposeAllConnections();
        this.connections = (ConcurrentStack<IConnection>) null;
        this.semaphore.Dispose();
        this.semaphore = (SemaphoreSlim) null;
        DefaultTrace.TraceInformation("Connection Pool Disposed");
      }
      this.isDisposed = true;
    }

    private void DisposeAllConnections()
    {
      IConnection result;
      while (this.connections.TryPop(out result))
        result.Close();
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (ConnectionPool));
    }

    public async Task<IConnection> GetOpenConnection(Guid activityId, Uri fullUri, string poolKey)
    {
      this.ThrowIfDisposed();
      int num = 0;
      while (num <= 3)
      {
        IConnection result;
        if (this.connections.TryPop(out result))
        {
          if (result.HasExpired())
          {
            result.Close();
          }
          else
          {
            if (result.ConfirmOpen())
              return result;
            ++num;
            result.Close();
          }
        }
        else
        {
          try
          {
            if (this.semaphore.CurrentCount == 0)
              DefaultTrace.TraceWarning("Too Many Concurrent Connections being opened. Current Pending Count: {0}", (object) this.ConcurrentConnectionMaxOpen);
            await this.semaphore.WaitAsync();
            return await this.connectionDispenser.OpenNewConnection(activityId, fullUri, poolKey);
          }
          finally
          {
            this.semaphore.Release();
          }
        }
      }
      this.DisposeAllConnections();
      throw new GoneException();
    }

    public void ReturnConnection(IConnection connection) => this.connections.Push(connection);
  }
}
