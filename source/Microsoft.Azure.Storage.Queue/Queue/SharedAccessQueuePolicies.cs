// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.SharedAccessQueuePolicies
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Queue
{
  public sealed class SharedAccessQueuePolicies : 
    IDictionary<string, SharedAccessQueuePolicy>,
    ICollection<KeyValuePair<string, SharedAccessQueuePolicy>>,
    IEnumerable<KeyValuePair<string, SharedAccessQueuePolicy>>,
    IEnumerable
  {
    private Dictionary<string, SharedAccessQueuePolicy> policies = new Dictionary<string, SharedAccessQueuePolicy>();

    public void Add(string key, SharedAccessQueuePolicy value) => this.policies.Add(key, value);

    public bool ContainsKey(string key) => this.policies.ContainsKey(key);

    public ICollection<string> Keys => (ICollection<string>) this.policies.Keys;

    public bool Remove(string key) => this.policies.Remove(key);

    public bool TryGetValue(string key, out SharedAccessQueuePolicy value) => this.policies.TryGetValue(key, out value);

    public ICollection<SharedAccessQueuePolicy> Values => (ICollection<SharedAccessQueuePolicy>) this.policies.Values;

    public SharedAccessQueuePolicy this[string key]
    {
      get => this.policies[key];
      set => this.policies[key] = value;
    }

    public void Add(KeyValuePair<string, SharedAccessQueuePolicy> item) => this.Add(item.Key, item.Value);

    public void Clear() => this.policies.Clear();

    public bool Contains(KeyValuePair<string, SharedAccessQueuePolicy> item)
    {
      SharedAccessQueuePolicy accessQueuePolicy;
      return this.TryGetValue(item.Key, out accessQueuePolicy) && string.Equals(SharedAccessQueuePolicy.PermissionsToString(item.Value.Permissions), SharedAccessQueuePolicy.PermissionsToString(accessQueuePolicy.Permissions), StringComparison.Ordinal);
    }

    public void CopyTo(
      KeyValuePair<string, SharedAccessQueuePolicy>[] array,
      int arrayIndex)
    {
      CommonUtility.AssertNotNull(nameof (array), (object) array);
      foreach (KeyValuePair<string, SharedAccessQueuePolicy> policy in this.policies)
        array[arrayIndex++] = policy;
    }

    public int Count => this.policies.Count;

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<string, SharedAccessQueuePolicy> item) => this.Contains(item) && this.Remove(item.Key);

    public IEnumerator<KeyValuePair<string, SharedAccessQueuePolicy>> GetEnumerator() => (IEnumerator<KeyValuePair<string, SharedAccessQueuePolicy>>) this.policies.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.policies).GetEnumerator();
  }
}
