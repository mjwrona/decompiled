// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConnectionPoolManager
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ConnectionPoolManager : IDisposable
  {
    private ConcurrentDictionary<string, ConnectionPool> connectionPools;
    private readonly IConnectionDispenser connectionDispenser;
    private int maxConcurrentConnectionOpenRequests;
    private bool isDisposed;

    public ConnectionPoolManager(
      IConnectionDispenser connectionDispenser,
      int maxConcurrentConnectionOpenRequests)
    {
      this.connectionPools = new ConcurrentDictionary<string, ConnectionPool>();
      this.connectionDispenser = connectionDispenser;
      this.maxConcurrentConnectionOpenRequests = maxConcurrentConnectionOpenRequests;
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
        foreach (KeyValuePair<string, ConnectionPool> connectionPool in this.connectionPools)
          connectionPool.Value.Dispose();
        this.connectionPools = (ConcurrentDictionary<string, ConnectionPool>) null;
        ((IDisposable) this.connectionDispenser).Dispose();
      }
      this.isDisposed = true;
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (ConnectionPoolManager));
    }

    public Task<IConnection> GetOpenConnection(Guid activityId, Uri fullUri)
    {
      this.ThrowIfDisposed();
      string poolKey = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) fullUri.Host, (object) fullUri.Port);
      return this.GetConnectionPool(poolKey).GetOpenConnection(activityId, fullUri, poolKey);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
    private ConnectionPool GetConnectionPool(string poolKey)
    {
      ConnectionPool connectionPool1 = (ConnectionPool) null;
      if (!this.connectionPools.TryGetValue(poolKey, out connectionPool1))
      {
        ConnectionPool connectionPool2 = new ConnectionPool(poolKey, this.connectionDispenser, this.maxConcurrentConnectionOpenRequests);
        connectionPool1 = this.connectionPools.GetOrAdd(poolKey, connectionPool2);
      }
      return connectionPool1;
    }

    public void ReturnToPool(IConnection connection)
    {
      ConnectionPool connectionPool;
      if (!this.connectionPools.TryGetValue(connection.PoolKey, out connectionPool))
        connection.Close();
      else
        connectionPool.ReturnConnection(connection);
    }
  }
}
