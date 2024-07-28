// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.IImsCacheOrchestrator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DefaultServiceImplementation(typeof (ImsCacheOrchestrator))]
  internal interface IImsCacheOrchestrator : IVssFrameworkService
  {
    IDictionary<K, V> GetObjects<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<V, bool> isCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject;

    IDictionary<K, V> GetObjects<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<V, bool> isCacheMiss,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject;

    IDictionary<K, V> GetObjectsAndRefreshLocal<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<V, bool> isCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject;
  }
}
