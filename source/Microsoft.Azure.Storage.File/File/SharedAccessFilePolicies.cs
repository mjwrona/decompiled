// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.SharedAccessFilePolicies
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.File
{
  public sealed class SharedAccessFilePolicies : 
    IDictionary<string, SharedAccessFilePolicy>,
    ICollection<KeyValuePair<string, SharedAccessFilePolicy>>,
    IEnumerable<KeyValuePair<string, SharedAccessFilePolicy>>,
    IEnumerable
  {
    private Dictionary<string, SharedAccessFilePolicy> policies = new Dictionary<string, SharedAccessFilePolicy>();

    public void Add(string key, SharedAccessFilePolicy value) => this.policies.Add(key, value);

    public bool ContainsKey(string key) => this.policies.ContainsKey(key);

    public ICollection<string> Keys => (ICollection<string>) this.policies.Keys;

    public bool Remove(string key) => this.policies.Remove(key);

    public bool TryGetValue(string key, out SharedAccessFilePolicy value) => this.policies.TryGetValue(key, out value);

    public ICollection<SharedAccessFilePolicy> Values => (ICollection<SharedAccessFilePolicy>) this.policies.Values;

    public SharedAccessFilePolicy this[string key]
    {
      get => this.policies[key];
      set => this.policies[key] = value;
    }

    public void Add(KeyValuePair<string, SharedAccessFilePolicy> item) => this.Add(item.Key, item.Value);

    public void Clear() => this.policies.Clear();

    public bool Contains(KeyValuePair<string, SharedAccessFilePolicy> item)
    {
      SharedAccessFilePolicy accessFilePolicy;
      return this.TryGetValue(item.Key, out accessFilePolicy) && string.Equals(SharedAccessFilePolicy.PermissionsToString(item.Value.Permissions), SharedAccessFilePolicy.PermissionsToString(accessFilePolicy.Permissions), StringComparison.Ordinal);
    }

    public void CopyTo(
      KeyValuePair<string, SharedAccessFilePolicy>[] array,
      int arrayIndex)
    {
      CommonUtility.AssertNotNull(nameof (array), (object) array);
      foreach (KeyValuePair<string, SharedAccessFilePolicy> policy in this.policies)
        array[arrayIndex++] = policy;
    }

    public int Count => this.policies.Count;

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<string, SharedAccessFilePolicy> item) => this.Contains(item) && this.Remove(item.Key);

    public IEnumerator<KeyValuePair<string, SharedAccessFilePolicy>> GetEnumerator() => (IEnumerator<KeyValuePair<string, SharedAccessFilePolicy>>) this.policies.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.policies).GetEnumerator();
  }
}
