// Decompiled with JetBrains decompiler
// Type: System.Threading.Async.REMOVE_ME.AsyncCache`2
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace System.Threading.Async.REMOVE_ME
{
  internal sealed class AsyncCache<TKey, TValue>
  {
    private readonly Func<TKey, Task<TValue>> valueFactory;
    private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> map;

    public AsyncCache(Func<TKey, Task<TValue>> valueFactory)
    {
      this.valueFactory = valueFactory;
      this.map = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
    }

    public Task<TValue> this[TKey key] => this.map.GetOrAdd(key, (Func<TKey, Lazy<Task<TValue>>>) (toAdd => new Lazy<Task<TValue>>((Func<Task<TValue>>) (() => this.valueFactory(toAdd))))).Value;

    public bool TryRemoveKey(TKey key) => this.map.TryRemove(key, out Lazy<Task<TValue>> _);
  }
}
