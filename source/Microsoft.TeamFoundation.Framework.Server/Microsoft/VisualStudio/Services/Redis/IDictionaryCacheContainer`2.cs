// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.IDictionaryCacheContainer`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Redis
{
  public interface IDictionaryCacheContainer<K, V> : ICacheContainer<K, V>
  {
    IEnumerable<V> Get(
      IVssRequestContext requestContext,
      IEnumerable<K> keys,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss);

    IEnumerable<KeyValuePair<V, bool>> TryGet(
      IVssRequestContext requestContext,
      IEnumerable<K> keys);

    void Invalidate(IVssRequestContext requestContext, IEnumerable<K> keys);

    IEnumerable<TimeSpan?> TimeToLive(IVssRequestContext requestContext, IEnumerable<K> keys);

    string GetCacheId();
  }
}
