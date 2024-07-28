// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MutableDictionaryCacheContainerFacade`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MutableDictionaryCacheContainerFacade<TKey, TValue> : 
    IMutableDictionaryCacheContainerFacade<TKey, TValue>,
    ICache<TKey, TValue>,
    IInvalidatableCache<TKey>
  {
    private readonly IMutableDictionaryCacheContainer<TKey, TValue> actualContainer;
    private readonly IVssRequestContext requestContext;

    public MutableDictionaryCacheContainerFacade(
      IVssRequestContext requestContext,
      IMutableDictionaryCacheContainer<TKey, TValue> actualContainer)
    {
      this.actualContainer = actualContainer;
      this.requestContext = requestContext;
    }

    public IEnumerable<TValue> Get(
      IEnumerable<TKey> keys,
      Func<IEnumerable<TKey>, IDictionary<TKey, TValue>> onCacheMiss)
    {
      return this.actualContainer.Get(this.requestContext, keys, onCacheMiss);
    }

    public TValue Get(TKey key, Func<TKey, TValue> onCacheMiss) => this.actualContainer.Get<TKey, TValue>(this.requestContext, key, onCacheMiss);

    public IEnumerable<KeyValuePair<TValue, bool>> TryGet(IEnumerable<TKey> keys) => this.actualContainer.TryGet(this.requestContext, keys);

    public bool TryGet(TKey key, out TValue value) => this.actualContainer.TryGet<TKey, TValue>(this.requestContext, key, out value);

    public bool Has(TKey key) => this.TryGet(key, out TValue _);

    public bool Set(TKey key, TValue val) => this.Set((IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>()
    {
      [key] = val
    });

    public void Invalidate(IEnumerable<TKey> keys) => this.actualContainer.Invalidate(this.requestContext, keys);

    public void Invalidate(TKey key) => this.actualContainer.Invalidate<TKey, TValue>(this.requestContext, key);

    public IEnumerable<TimeSpan?> TimeToLive(IEnumerable<TKey> keys) => this.actualContainer.TimeToLive(this.requestContext, keys);

    public string GetCacheId() => this.actualContainer.GetCacheId();

    public bool Set(IDictionary<TKey, TValue> items) => this.actualContainer.Set(this.requestContext, items);

    public IEnumerable<TValue> IncrementBy(IEnumerable<KeyValuePair<TKey, TValue>> items) => this.actualContainer.IncrementBy(this.requestContext, items);

    public TValue IncrementBy(TKey key, TValue addition) => this.actualContainer.IncrementBy<TKey, TValue>(this.requestContext, key, addition);
  }
}
