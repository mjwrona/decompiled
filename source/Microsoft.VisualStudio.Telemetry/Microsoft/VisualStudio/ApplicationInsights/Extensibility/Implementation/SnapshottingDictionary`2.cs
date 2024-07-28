// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.SnapshottingDictionary`2
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class SnapshottingDictionary<TKey, TValue> : 
    SnapshottingCollection<KeyValuePair<TKey, TValue>, IDictionary<TKey, TValue>>,
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
  {
    public SnapshottingDictionary()
      : base((IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>())
    {
    }

    public ICollection<TKey> Keys => this.GetSnapshot().Keys;

    public ICollection<TValue> Values => this.GetSnapshot().Values;

    public TValue this[TKey key]
    {
      get => this.GetSnapshot()[key];
      set
      {
        lock (this.Collection)
        {
          this.Collection[key] = value;
          this.snapshot = (IDictionary<TKey, TValue>) null;
        }
      }
    }

    public void Add(TKey key, TValue value)
    {
      lock (this.Collection)
      {
        this.Collection.Add(key, value);
        this.snapshot = (IDictionary<TKey, TValue>) null;
      }
    }

    public bool ContainsKey(TKey key) => this.GetSnapshot().ContainsKey(key);

    public bool Remove(TKey key)
    {
      lock (this.Collection)
      {
        int num = this.Collection.Remove(key) ? 1 : 0;
        if (num != 0)
          this.snapshot = (IDictionary<TKey, TValue>) null;
        return num != 0;
      }
    }

    public bool TryGetValue(TKey key, out TValue value) => this.GetSnapshot().TryGetValue(key, out value);

    protected override sealed IDictionary<TKey, TValue> CreateSnapshot(
      IDictionary<TKey, TValue> collection)
    {
      return (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>(collection);
    }
  }
}
