// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RedisCacheBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Redis;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RedisCacheBootstrapper<TKey, TVal> : 
    IBootstrapper<IFactory<IMutableDictionaryCacheContainerFacade<TKey, TVal>>>
  {
    private readonly Guid namespaceGuid;
    private readonly ContainerSettings cacheContainerSettings;
    private readonly IVssRequestContext requestContext;

    public RedisCacheBootstrapper(
      IVssRequestContext requestContext,
      Guid namespaceGuid,
      ContainerSettings cacheContainerSettings)
    {
      this.namespaceGuid = namespaceGuid;
      this.cacheContainerSettings = cacheContainerSettings;
      this.requestContext = requestContext;
    }

    public IFactory<IMutableDictionaryCacheContainerFacade<TKey, TVal>> Bootstrap() => (IFactory<IMutableDictionaryCacheContainerFacade<TKey, TVal>>) new RedisCacheFactory<TKey, TVal, RedisCacheBootstrapper<TKey, TVal>.RedisCacheSecurityToken>(this.requestContext, (IRedisServiceFacade) new RedisServiceFacade(this.requestContext.GetService<IRedisCacheService>(), this.requestContext), this.namespaceGuid, this.cacheContainerSettings);

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
