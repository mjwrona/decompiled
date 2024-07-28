// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.CacheContainerExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Redis
{
  public static class CacheContainerExtensions
  {
    public static V Get<K, V>(
      this IDictionaryCacheContainer<K, V> container,
      IVssRequestContext requestContext,
      K key,
      Func<K, V> onCacheMiss)
    {
      return container.Get(requestContext, (IEnumerable<K>) new K[1]
      {
        key
      }, (Func<IEnumerable<K>, IDictionary<K, V>>) (keys =>
      {
        K k = keys.Single<K>();
        V v = onCacheMiss(k);
        return (IDictionary<K, V>) new Dictionary<K, V>()
        {
          {
            k,
            v
          }
        };
      })).Single<V>();
    }

    public static bool TryGet<K, V>(
      this IDictionaryCacheContainer<K, V> container,
      IVssRequestContext requestContext,
      K key,
      out V value)
    {
      KeyValuePair<V, bool> keyValuePair = container.TryGet(requestContext, (IEnumerable<K>) new K[1]
      {
        key
      }).Single<KeyValuePair<V, bool>>();
      value = keyValuePair.Key;
      return keyValuePair.Value;
    }

    public static void Invalidate<K, V>(
      this IDictionaryCacheContainer<K, V> container,
      IVssRequestContext requestContext,
      K key)
    {
      container.Invalidate(requestContext, (IEnumerable<K>) new K[1]
      {
        key
      });
    }

    public static V IncrementBy<K, V>(
      this IMutableCacheContainer<K, V> container,
      IVssRequestContext requestContext,
      K key,
      V addition)
    {
      return container.IncrementBy(requestContext, (IEnumerable<KeyValuePair<K, V>>) new KeyValuePair<K, V>[1]
      {
        new KeyValuePair<K, V>(key, addition)
      }).Single<V>();
    }
  }
}
