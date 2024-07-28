// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.ConnectionPool
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal class ConnectionPool : IRedisConnectionPool, IDisposable
  {
    private readonly RedisConfiguration m_configuration;
    private readonly ConnectionPoolSettings m_defaultSettings;
    private readonly Microsoft.VisualStudio.Services.Redis.Tracer m_tracer;
    private readonly RedisReconnectSync m_reconnectSync;
    private readonly object m_lock = new object();
    private RedisConnection[] m_connections;
    private IPartitionPolicy m_partitionPolicy;
    internal const int MaxPoolSize = 256;

    internal IPartitionPolicy PartitionPolicy => this.m_partitionPolicy;

    internal RedisConnection[] Connections => this.m_connections;

    public ConnectionPool(
      IVssRequestContext requestContext,
      RedisConfiguration configuration,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      RedisReconnectSync sync,
      string name = "$Default",
      ConnectionPoolSettings defaultSettings = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Name = name;
      this.m_configuration = configuration;
      this.m_tracer = tracer;
      this.m_reconnectSync = sync;
      this.m_defaultSettings = defaultSettings ?? new ConnectionPoolSettings();
      this.m_partitionPolicy = this.CreatePolicy(requestContext, configuration, this.Name, tracer, this.m_defaultSettings.PartitionPolicy);
      int configuredPoolSize = ConnectionPool.GetConfiguredPoolSize(requestContext, configuration, this.Name, this.m_defaultSettings.PoolSize);
      this.m_connections = new RedisConnection[configuredPoolSize];
      for (int connectionIndex = 0; connectionIndex < configuredPoolSize; ++connectionIndex)
      {
        string connectionId = ConnectionPool.GetConnectionId(this.Name, connectionIndex);
        this.m_connections[connectionIndex] = RedisConnection.Create(requestContext, tracer, this.m_reconnectSync, configuration, this.Name, connectionId, this.m_defaultSettings);
      }
    }

    public string Name { get; }

    public int Size
    {
      get
      {
        lock (this.m_lock)
          return this.m_connections.Length;
      }
    }

    public void Dispose()
    {
      for (int index = 0; index < this.m_connections.Length; ++index)
      {
        if (this.m_connections[index] != null)
          this.m_connections[index].Dispose();
      }
    }

    public void UpdateSettings(IVssRequestContext requestContext)
    {
      this.m_partitionPolicy = this.CreatePolicy(requestContext, this.m_configuration, this.Name, this.m_tracer, this.m_defaultSettings.PartitionPolicy);
      this.m_tracer.RedisError(requestContext, string.Format("Redis connection pool policy set to {0}", (object) this.m_partitionPolicy));
      for (int index = 0; index < this.m_connections.Length; ++index)
        this.m_connections[index].UpdateSettings(requestContext);
      int configuredPoolSize = ConnectionPool.GetConfiguredPoolSize(requestContext, this.m_configuration, this.Name, this.m_defaultSettings.PoolSize);
      if (configuredPoolSize == this.m_connections.Length)
        return;
      RedisConnection[] connections = this.m_connections;
      RedisConnection[] destinationArray = new RedisConnection[configuredPoolSize];
      Array.Copy((Array) connections, (Array) destinationArray, Math.Min(connections.Length, destinationArray.Length));
      if (destinationArray.Length > connections.Length)
      {
        this.m_tracer.RedisError(requestContext, string.Format("Expanding Redis connection pool {0} -> {1}", (object) connections.Length, (object) destinationArray.Length));
        for (int length = connections.Length; length < destinationArray.Length; ++length)
        {
          string connectionId = ConnectionPool.GetConnectionId(this.Name, length);
          destinationArray[length] = RedisConnection.Create(requestContext, this.m_tracer, this.m_reconnectSync, this.m_configuration, this.Name, connectionId, this.m_defaultSettings);
        }
        lock (this.m_lock)
          this.m_connections = destinationArray;
      }
      if (destinationArray.Length >= connections.Length)
        return;
      this.m_tracer.RedisError(requestContext, string.Format("Shrinking Redis connection pool {0} -> {1}", (object) connections.Length, (object) destinationArray.Length));
      lock (this.m_lock)
        this.m_connections = destinationArray;
      for (int length = destinationArray.Length; length < connections.Length; ++length)
        connections[length].Dispose();
    }

    public IRedisConnection AcquireConnection(IVssRequestContext requestContext, Guid namespaceId)
    {
      int slot = this.m_partitionPolicy.GetSlot(requestContext, namespaceId);
      RedisConnection connection;
      lock (this.m_lock)
      {
        connection = this.m_connections[Math.Abs(slot % this.m_connections.Length)];
        connection.AcquireConnection();
      }
      return (IRedisConnection) connection;
    }

    internal virtual IPartitionPolicy CreatePolicy(
      IVssRequestContext requestContext,
      RedisConfiguration configuration,
      string poolName,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      IPartitionPolicy defaultPolicy)
    {
      IPartitionPolicy partitionPolicy = (IPartitionPolicy) null;
      string typeName = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) configuration.Keys.PartitionPolicyTypeName(poolName), false, (string) null);
      if (!string.IsNullOrEmpty(typeName))
      {
        try
        {
          partitionPolicy = (IPartitionPolicy) Activator.CreateInstance((string) null, typeName).Unwrap();
        }
        catch (Exception ex)
        {
          tracer.RedisError(requestContext, string.Format("Failed creating partition policy {0}, exception = {1}", (object) typeName, (object) ex));
        }
      }
      return partitionPolicy ?? defaultPolicy;
    }

    private static string GetConnectionId(string name, int connectionIndex) => string.IsNullOrEmpty(name) || name.Equals("$Default", StringComparison.OrdinalIgnoreCase) ? string.Format("RedisConnection#{0}", (object) connectionIndex) : string.Format("{0}/RedisConnection#{1}", (object) name, (object) connectionIndex);

    private static int GetConfiguredPoolSize(
      IVssRequestContext requestContext,
      RedisConfiguration configuration,
      string poolName,
      int defaultSize)
    {
      return Math.Min(Math.Max(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) configuration.Keys.ConnectionPoolSize(poolName), defaultSize), 1), 256);
    }
  }
}
