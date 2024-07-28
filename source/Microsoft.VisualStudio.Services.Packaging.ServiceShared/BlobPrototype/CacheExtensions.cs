// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CacheExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class CacheExtensions
  {
    public static TVal GetOrAddNonAtomic<TKey, TVal>(
      this ICache<TKey, TVal> cache,
      TKey key,
      Func<TVal> valueFactory)
    {
      TVal val1;
      if (cache.TryGet(key, out val1))
        return val1;
      TVal val2 = valueFactory();
      cache.Set(key, val2);
      return val2;
    }

    public static async Task<TVal> GetOrAddNonAtomicAsync<TKey, TVal>(
      this ICache<TKey, TVal> cache,
      TKey key,
      Func<Task<TVal>> valueFactory)
    {
      TVal val;
      if (cache.TryGet(key, out val))
        return val;
      val = await valueFactory();
      cache.Set(key, val);
      return val;
    }
  }
}
