// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.StubMutableDictionaryCacheContainer`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class StubMutableDictionaryCacheContainer<K, V> : 
    IMutableDictionaryCacheContainer<K, V>,
    IDictionaryCacheContainer<K, V>,
    ICacheContainer<K, V>,
    IMutableCacheContainer<K, V>
  {
    IEnumerable<V> IDictionaryCacheContainer<K, V>.Get(
      IVssRequestContext requestContext,
      IEnumerable<K> keys,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss)
    {
      IDictionary<K, V> values = onCacheMiss(keys);
      V v;
      return keys.Select<K, V>((Func<K, V>) (k => !values.TryGetValue(k, out v) ? default (V) : v));
    }

    IEnumerable<KeyValuePair<V, bool>> IDictionaryCacheContainer<K, V>.TryGet(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      return keys.Select<K, KeyValuePair<V, bool>>((Func<K, KeyValuePair<V, bool>>) (key => new KeyValuePair<V, bool>(default (V), false)));
    }

    bool IMutableCacheContainer<K, V>.Set(
      IVssRequestContext requestContext,
      IDictionary<K, V> items)
    {
      return false;
    }

    IEnumerable<V> IMutableCacheContainer<K, V>.IncrementBy(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<K, V>> items)
    {
      return items.Select<KeyValuePair<K, V>, V>((Func<KeyValuePair<K, V>, V>) (x => x.Value));
    }

    void IDictionaryCacheContainer<K, V>.Invalidate(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
    }

    IEnumerable<TimeSpan?> IDictionaryCacheContainer<K, V>.TimeToLive(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      return keys.Select<K, TimeSpan?>((Func<K, TimeSpan?>) (key => new TimeSpan?()));
    }

    IMutableCacheContainer<K, V> ICacheContainer<K, V>.AsMutable() => (IMutableCacheContainer<K, V>) this;

    public string GetCacheId() => (string) null;
  }
}
