// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.SafeSet`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class SafeSet<T>
  {
    private readonly ConcurrentDictionary<T, object> _items;

    public SafeSet() => this._items = new ConcurrentDictionary<T, object>();

    public SafeSet(IEqualityComparer<T> comparer) => this._items = new ConcurrentDictionary<T, object>(comparer);

    public SafeSet(IEnumerable<T> items) => this._items = new ConcurrentDictionary<T, object>(items.Select<T, KeyValuePair<T, object>>((Func<T, KeyValuePair<T, object>>) (x => new KeyValuePair<T, object>(x, (object) null))));

    public ICollection<T> GetSnapshot() => this._items.Keys;

    public bool Contains(T item) => this._items.ContainsKey(item);

    public bool Add(T item) => this._items.TryAdd(item, (object) null);

    public bool Remove(T item) => this._items.TryRemove(item, out object _);

    public bool Any() => this._items.Any<KeyValuePair<T, object>>();

    public long Count => (long) this._items.Count;
  }
}
