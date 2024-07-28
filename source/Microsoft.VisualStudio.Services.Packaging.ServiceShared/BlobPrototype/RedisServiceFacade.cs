// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RedisServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RedisServiceFacade : IRedisServiceFacade
  {
    private readonly IRedisCacheService redisCacheService;
    private readonly IVssRequestContext requestContext;

    public RedisServiceFacade(
      IRedisCacheService redisCacheService,
      IVssRequestContext requestContext)
    {
      this.redisCacheService = redisCacheService;
      this.requestContext = requestContext;
    }

    public IMutableDictionaryCacheContainerFacade<TKey, TVal> GetVolatileDictionaryContainer<TKey, TVal, TSecurity>(
      Guid namespaceId,
      ContainerSettings settings = null)
    {
      return !this.redisCacheService.IsEnabled(this.requestContext) ? (IMutableDictionaryCacheContainerFacade<TKey, TVal>) null : (IMutableDictionaryCacheContainerFacade<TKey, TVal>) new MutableDictionaryCacheContainerFacade<TKey, TVal>(this.requestContext, this.redisCacheService.GetVolatileDictionaryContainer<TKey, TVal, TSecurity>(this.requestContext, namespaceId, settings));
    }

    public bool IsEnabled() => this.redisCacheService.IsEnabled(this.requestContext);
  }
}
