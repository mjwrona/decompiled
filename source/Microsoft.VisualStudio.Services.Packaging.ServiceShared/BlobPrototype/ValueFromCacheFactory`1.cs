// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ValueFromCacheFactory`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ValueFromCacheFactory<T> : IFactory<T>
  {
    private readonly string key;
    private readonly ICache<string, object> cache;
    private readonly T valueIfNotFound;

    public ValueFromCacheFactory(string key, ICache<string, object> cache, T valueIfNotFound = null)
    {
      this.key = key;
      this.cache = cache;
      this.valueIfNotFound = valueIfNotFound;
    }

    public T Get()
    {
      object val;
      return this.cache.TryGet(this.key, out val) && val is T obj ? obj : this.valueIfNotFound;
    }
  }
}
