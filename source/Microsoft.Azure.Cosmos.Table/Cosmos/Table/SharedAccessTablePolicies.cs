// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.SharedAccessTablePolicies
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class SharedAccessTablePolicies : 
    IDictionary<string, SharedAccessTablePolicy>,
    ICollection<KeyValuePair<string, SharedAccessTablePolicy>>,
    IEnumerable<KeyValuePair<string, SharedAccessTablePolicy>>,
    IEnumerable
  {
    private readonly Dictionary<string, SharedAccessTablePolicy> policies = new Dictionary<string, SharedAccessTablePolicy>();

    public void Add(string key, SharedAccessTablePolicy value) => this.policies.Add(key, value);

    public bool ContainsKey(string key) => this.policies.ContainsKey(key);

    public ICollection<string> Keys => (ICollection<string>) this.policies.Keys;

    public bool Remove(string key) => this.policies.Remove(key);

    public bool TryGetValue(string key, out SharedAccessTablePolicy value) => this.policies.TryGetValue(key, out value);

    public ICollection<SharedAccessTablePolicy> Values => (ICollection<SharedAccessTablePolicy>) this.policies.Values;

    public SharedAccessTablePolicy this[string key]
    {
      get => this.policies[key];
      set => this.policies[key] = value;
    }

    public void Add(KeyValuePair<string, SharedAccessTablePolicy> item) => this.Add(item.Key, item.Value);

    public void Clear() => this.policies.Clear();

    public bool Contains(KeyValuePair<string, SharedAccessTablePolicy> item)
    {
      SharedAccessTablePolicy accessTablePolicy;
      return this.TryGetValue(item.Key, out accessTablePolicy) && string.Equals(SharedAccessTablePolicy.PermissionsToString(item.Value.Permissions), SharedAccessTablePolicy.PermissionsToString(accessTablePolicy.Permissions), StringComparison.Ordinal);
    }

    public void CopyTo(
      KeyValuePair<string, SharedAccessTablePolicy>[] array,
      int arrayIndex)
    {
      CommonUtility.AssertNotNull(nameof (array), (object) array);
      foreach (KeyValuePair<string, SharedAccessTablePolicy> policy in this.policies)
        array[arrayIndex++] = policy;
    }

    public int Count => this.policies.Count;

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<string, SharedAccessTablePolicy> item) => this.Contains(item) && this.Remove(item.Key);

    public IEnumerator<KeyValuePair<string, SharedAccessTablePolicy>> GetEnumerator() => (IEnumerator<KeyValuePair<string, SharedAccessTablePolicy>>) this.policies.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.policies).GetEnumerator();
  }
}
