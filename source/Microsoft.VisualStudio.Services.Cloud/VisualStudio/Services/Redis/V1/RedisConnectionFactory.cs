// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V1.RedisConnectionFactory
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Redis.V1
{
  public class RedisConnectionFactory : IRedisConnectionFactory
  {
    private readonly RedisCacheManager m_connectionManager;
    private readonly string m_poolName;
    private readonly Guid m_namespaceId;

    internal RedisConnectionFactory(
      RedisCacheManager connectionManager,
      string poolName,
      Guid namespaceId)
    {
      this.m_connectionManager = connectionManager;
      this.m_poolName = poolName;
      this.m_namespaceId = namespaceId;
    }

    public static IRedisConnectionFactory Create(
      IVssRequestContext requestContext,
      string poolName = "$Default",
      Guid namespaceId = default (Guid))
    {
      return (IRedisConnectionFactory) new RedisConnectionFactory(requestContext.To(TeamFoundationHostType.Deployment).GetService<RedisCacheManager>(), poolName, namespaceId);
    }

    public IRedisConnection CreateConnection(IVssRequestContext requestContext) => this.m_connectionManager.CreateConnection(requestContext, this.m_poolName, this.m_namespaceId);
  }
}
