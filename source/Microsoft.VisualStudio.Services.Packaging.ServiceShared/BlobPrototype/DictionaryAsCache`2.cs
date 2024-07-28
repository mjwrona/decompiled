// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DictionaryAsCache`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class DictionaryAsCache<TKey, TVal> : ICache<TKey, TVal>, IInvalidatableCache<TKey>
  {
    private IDictionary<TKey, TVal> dict;

    public DictionaryAsCache(IDictionary<TKey, TVal> dict = null) => this.dict = dict ?? (IDictionary<TKey, TVal>) new Dictionary<TKey, TVal>();

    public bool TryGet(TKey key, out TVal val) => this.dict.TryGetValue(key, out val);

    public bool Has(TKey key) => this.dict.ContainsKey(key);

    public bool Set(TKey key, TVal val)
    {
      this.dict[key] = val;
      return true;
    }

    public void Invalidate(TKey key) => this.dict.Remove(key);
  }
}
