// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.StubRedisCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal sealed class StubRedisCacheService : IRedisCacheService, IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool IsEnabled(IVssRequestContext requestContext) => false;

    public IMutableDictionaryCacheContainer<K, V> GetVolatileDictionaryContainer<K, V, S>(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings = null)
    {
      return (IMutableDictionaryCacheContainer<K, V>) new StubMutableDictionaryCacheContainer<K, V>();
    }

    public IWindowedDictionaryCacheContainer<K> GetWindowedDictionaryContainer<K, S>(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings = null)
    {
      return (IWindowedDictionaryCacheContainer<K>) new StubWindowedDictionaryCacheContainer<K>();
    }
  }
}
