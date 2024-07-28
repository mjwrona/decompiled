// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.RedisRegistryKeys
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class RedisRegistryKeys
  {
    public RedisRegistryKeys()
      : this("/Configuration/Caching/Redis")
    {
    }

    public RedisRegistryKeys(string rootPath)
    {
      this.ConfigurationRootPath = rootPath.TrimEnd('/');
      this.ConnectionPoolsRootPath = this.ConfigurationRootPath + "/ConnectionPools";
      this.NamespacesRootPath = this.ConfigurationRootPath + "/Namespaces";
    }

    public string ConfigurationRootPath { get; }

    public string ConnectionPoolsRootPath { get; }

    public string NamespacesRootPath { get; }

    public string ConnectionString => this.ConfigurationRootPath + "/ConnectionString";

    public string ConnectionTimeout => this.ConfigurationRootPath + "/ConnectionTimeout";

    public string ConnectionRetries => this.ConfigurationRootPath + "/ConnectionRetries";

    public string ReconnectDeltaBackoff => this.ConfigurationRootPath + "/ReconnectDeltaBackoff";

    public string ReconnectMaxBackoff => this.ConfigurationRootPath + "/ReconnectMaxBackoff";

    public string MonitoringInterval => this.ConfigurationRootPath + "/MonitoringInterval";

    public string MemoryCacheSize => this.ConfigurationRootPath + "/MemoryCacheSize";

    public string RequestTimeout => this.ConfigurationRootPath + "/RequestTimeout";

    public string NoThrowMode() => this.ConfigurationRootPath + "/NoThrowMode";

    public string AllowBatching() => this.ConfigurationRootPath + "/AllowBatching";

    public string WarmupTimeout() => this.ConfigurationRootPath + "/WarmupTimeout";

    public string BatchingRadixBackoff => this.ConfigurationRootPath + "/BatchingRadixBackoff";

    public string UseDefaultBacklogPolicy => this.ConfigurationRootPath + "/UseDefaultBacklogPolicy";

    public string ConnectionPool(Guid namespaceId) => this.GetNamespaceValue(namespaceId, nameof (ConnectionPool));

    public string ConnectionPoolSize(string poolName = null) => this.GetConnectionPoolValue(poolName, nameof (ConnectionPoolSize));

    public string PartitionPolicyTypeName(string poolName = null) => this.GetConnectionPoolValue(poolName, nameof (PartitionPolicyTypeName));

    public string MaxMessageSize(string poolName = null) => this.GetConnectionPoolValue(poolName, nameof (MaxMessageSize));

    public string RetryCount(string poolName = null) => this.GetConnectionPoolValue(poolName, nameof (RetryCount));

    public string NeedsPubSub(string poolName = null) => this.GetConnectionPoolValue(poolName, nameof (NeedsPubSub));

    public string MaxFailuresPerRequest(string poolName = null) => this.GetConnectionPoolValue(poolName, nameof (MaxFailuresPerRequest));

    public string KeyExpiry(Guid cacheId) => this.ConfigurationRootPath + "/" + cacheId.ToString() + "/KeyExpiry";

    private string GetConnectionPoolValue(string poolName, string key) => !string.IsNullOrEmpty(poolName) && !poolName.Equals("$Default", StringComparison.OrdinalIgnoreCase) ? this.ConnectionPoolsRootPath.TrimEnd('/') + "/" + poolName + "/" + key : this.ConfigurationRootPath.TrimEnd('/') + "/" + key;

    internal string GetNamespaceValue(Guid namespaceId, string key) => string.Format("{0}/{1}/{2}", (object) this.NamespacesRootPath.TrimEnd('/'), (object) namespaceId, (object) key);
  }
}
