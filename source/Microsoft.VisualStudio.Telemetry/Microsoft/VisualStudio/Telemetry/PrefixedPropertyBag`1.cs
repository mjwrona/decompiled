// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.PrefixedPropertyBag`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class PrefixedPropertyBag<TValue> : 
    IDictionary<string, TValue>,
    ICollection<KeyValuePair<string, TValue>>,
    IEnumerable<KeyValuePair<string, TValue>>,
    IEnumerable
  {
    private readonly string prefix;
    private readonly IDictionary<string, TValue> withPrefix;

    public PrefixedPropertyBag(IDictionary<string, TValue> backingDictionary, string prefix)
    {
      this.withPrefix = backingDictionary;
      this.prefix = prefix;
    }

    public TValue this[string key]
    {
      get => this.withPrefix[this.GetFullPropertyName(key)];
      set => this.withPrefix[this.GetFullPropertyName(key)] = value;
    }

    public ICollection<string> Keys => (ICollection<string>) this.withPrefix.Select<KeyValuePair<string, TValue>, string>((Func<KeyValuePair<string, TValue>, string>) (kv => this.GetShortPropertyName(kv.Key))).ToList<string>();

    public ICollection<TValue> Values => this.withPrefix.Values;

    public int Count => this.withPrefix.Count;

    public bool IsReadOnly => this.withPrefix.IsReadOnly;

    public void Add(string key, TValue value) => this.withPrefix.Add(this.GetFullPropertyName(key), value);

    public void Add(KeyValuePair<string, TValue> item) => this.withPrefix.Add(this.GetFullPropertyName(item));

    public void Clear() => this.withPrefix.Clear();

    public bool Contains(KeyValuePair<string, TValue> item) => this.withPrefix.Contains(this.GetFullPropertyName(item));

    public bool ContainsKey(string key) => this.withPrefix.ContainsKey(this.GetFullPropertyName(key));

    public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, TValue>>) new Dictionary<string, TValue>((IDictionary<string, TValue>) this, (IEqualityComparer<string>) null)).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
    {
      foreach (KeyValuePair<string, TValue> pair in (IEnumerable<KeyValuePair<string, TValue>>) this.withPrefix)
        yield return this.GetShortPropertyName(pair);
    }

    public bool Remove(string key) => this.withPrefix.Remove(this.GetFullPropertyName(key));

    public bool Remove(KeyValuePair<string, TValue> item) => ((ICollection<KeyValuePair<string, TValue>>) this.withPrefix).Remove(this.GetFullPropertyName(item));

    public bool TryGetValue(string key, out TValue value) => this.withPrefix.TryGetValue(this.GetFullPropertyName(key), out value);

    IEnumerator IEnumerable.GetEnumerator()
    {
      IEnumerator<KeyValuePair<string, TValue>> enumerator = this.GetEnumerator();
      while (enumerator.MoveNext())
        yield return (object) enumerator.Current;
    }

    internal void AddPrefixed(string key, TValue value)
    {
      this.AssertIsPrefixed(key);
      this.withPrefix[key] = value;
    }

    internal void RemovePrefixed(string key)
    {
      this.AssertIsPrefixed(key);
      this.withPrefix.Remove(key);
    }

    internal TValue GetPrefixed(string key)
    {
      this.AssertIsPrefixed(key);
      return this.withPrefix[key];
    }

    internal IEnumerable<KeyValuePair<string, TValue>> PrefixedEnumerable => (IEnumerable<KeyValuePair<string, TValue>>) this.withPrefix;

    internal void AddRangePrefixed(
      IEnumerable<KeyValuePair<string, TValue>> source,
      bool forceUpdate = true)
    {
      foreach (KeyValuePair<string, TValue> keyValuePair in source)
      {
        this.AssertIsPrefixed(keyValuePair.Key);
        if (forceUpdate || !this.withPrefix.ContainsKey(keyValuePair.Key))
          this.withPrefix[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    private void AssertIsPrefixed(string key)
    {
      if (key == null || !key.StartsWith(this.prefix))
        throw new ArgumentException("Key name must start with '" + this.prefix + "'");
    }

    private string GetFullPropertyName(string shortName) => this.prefix == null ? shortName : this.prefix + shortName;

    private KeyValuePair<string, TValue> GetFullPropertyName(KeyValuePair<string, TValue> pair) => new KeyValuePair<string, TValue>(this.GetFullPropertyName(pair.Key), pair.Value);

    private string GetShortPropertyName(string fullName) => this.prefix == null ? fullName : fullName.Substring(this.prefix.Length);

    private KeyValuePair<string, TValue> GetShortPropertyName(KeyValuePair<string, TValue> pair) => new KeyValuePair<string, TValue>(this.GetShortPropertyName(pair.Key), pair.Value);
  }
}
