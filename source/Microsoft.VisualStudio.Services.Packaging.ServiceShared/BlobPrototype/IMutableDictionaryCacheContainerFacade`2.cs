// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IMutableDictionaryCacheContainerFacade`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface IMutableDictionaryCacheContainerFacade<TKey, TValue> : 
    ICache<TKey, TValue>,
    IInvalidatableCache<TKey>
  {
    IEnumerable<TValue> Get(
      IEnumerable<TKey> keys,
      Func<IEnumerable<TKey>, IDictionary<TKey, TValue>> onCacheMiss);

    TValue Get(TKey key, Func<TKey, TValue> onCacheMiss);

    IEnumerable<KeyValuePair<TValue, bool>> TryGet(IEnumerable<TKey> keys);

    void Invalidate(IEnumerable<TKey> keys);

    IEnumerable<TimeSpan?> TimeToLive(IEnumerable<TKey> keys);

    string GetCacheId();

    bool Set(IDictionary<TKey, TValue> items);

    IEnumerable<TValue> IncrementBy(IEnumerable<KeyValuePair<TKey, TValue>> items);

    TValue IncrementBy(TKey key, TValue addition);
  }
}
