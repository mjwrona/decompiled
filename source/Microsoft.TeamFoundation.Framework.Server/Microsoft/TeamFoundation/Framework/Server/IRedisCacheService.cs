// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IRedisCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Redis;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Redis.RedisCacheService, Microsoft.VisualStudio.Services.Cloud")]
  public interface IRedisCacheService : IVssFrameworkService
  {
    bool IsEnabled(IVssRequestContext requestContext);

    IMutableDictionaryCacheContainer<K, V> GetVolatileDictionaryContainer<K, V, S>(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings = null);

    IWindowedDictionaryCacheContainer<K> GetWindowedDictionaryContainer<K, S>(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings = null);
  }
}
