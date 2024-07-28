// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RedisCacheFactory`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Redis;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RedisCacheFactory<TKey, TVal, TSecurity> : 
    IFactory<IMutableDictionaryCacheContainerFacade<TKey, TVal>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IRedisServiceFacade redisServiceFacade;
    private readonly Guid namespaceGuid;
    private readonly ContainerSettings cacheContainerSettings;

    public RedisCacheFactory(
      IVssRequestContext requestContext,
      IRedisServiceFacade redisServiceFacade,
      Guid namespaceGuid,
      ContainerSettings cacheContainerSettings)
    {
      this.requestContext = requestContext;
      this.redisServiceFacade = redisServiceFacade;
      this.namespaceGuid = namespaceGuid;
      this.cacheContainerSettings = cacheContainerSettings;
    }

    public IMutableDictionaryCacheContainerFacade<TKey, TVal> Get() => this.redisServiceFacade.IsEnabled() ? this.redisServiceFacade.GetVolatileDictionaryContainer<TKey, TVal, TSecurity>(this.namespaceGuid, this.cacheContainerSettings) : (IMutableDictionaryCacheContainerFacade<TKey, TVal>) null;
  }
}
