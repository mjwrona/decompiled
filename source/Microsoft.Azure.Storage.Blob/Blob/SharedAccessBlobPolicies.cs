// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.SharedAccessBlobPolicies
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class SharedAccessBlobPolicies : 
    IDictionary<string, SharedAccessBlobPolicy>,
    ICollection<KeyValuePair<string, SharedAccessBlobPolicy>>,
    IEnumerable<KeyValuePair<string, SharedAccessBlobPolicy>>,
    IEnumerable
  {
    private Dictionary<string, SharedAccessBlobPolicy> policies = new Dictionary<string, SharedAccessBlobPolicy>();

    public void Add(string key, SharedAccessBlobPolicy value) => this.policies.Add(key, value);

    public bool ContainsKey(string key) => this.policies.ContainsKey(key);

    public ICollection<string> Keys => (ICollection<string>) this.policies.Keys;

    public bool Remove(string key) => this.policies.Remove(key);

    public bool TryGetValue(string key, out SharedAccessBlobPolicy value) => this.policies.TryGetValue(key, out value);

    public ICollection<SharedAccessBlobPolicy> Values => (ICollection<SharedAccessBlobPolicy>) this.policies.Values;

    public SharedAccessBlobPolicy this[string key]
    {
      get => this.policies[key];
      set => this.policies[key] = value;
    }

    public void Add(KeyValuePair<string, SharedAccessBlobPolicy> item) => this.Add(item.Key, item.Value);

    public void Clear() => this.policies.Clear();

    public bool Contains(KeyValuePair<string, SharedAccessBlobPolicy> item)
    {
      SharedAccessBlobPolicy accessBlobPolicy;
      return this.TryGetValue(item.Key, out accessBlobPolicy) && string.Equals(SharedAccessBlobPolicy.PermissionsToString(item.Value.Permissions), SharedAccessBlobPolicy.PermissionsToString(accessBlobPolicy.Permissions), StringComparison.Ordinal);
    }

    public void CopyTo(
      KeyValuePair<string, SharedAccessBlobPolicy>[] array,
      int arrayIndex)
    {
      CommonUtility.AssertNotNull(nameof (array), (object) array);
      foreach (KeyValuePair<string, SharedAccessBlobPolicy> policy in this.policies)
        array[arrayIndex++] = policy;
    }

    public int Count => this.policies.Count;

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<string, SharedAccessBlobPolicy> item) => this.Contains(item) && this.Remove(item.Key);

    public IEnumerator<KeyValuePair<string, SharedAccessBlobPolicy>> GetEnumerator() => (IEnumerator<KeyValuePair<string, SharedAccessBlobPolicy>>) this.policies.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.policies).GetEnumerator();
  }
}
