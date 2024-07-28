// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ServiceCollection : 
    IVssFrameworkServiceCollection,
    IDictionary<string, VssFrameworkServiceDescriptor>,
    ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>,
    IEnumerable<KeyValuePair<string, VssFrameworkServiceDescriptor>>,
    IEnumerable
  {
    private readonly Dictionary<string, VssFrameworkServiceDescriptor> _descriptors = new Dictionary<string, VssFrameworkServiceDescriptor>();

    public VssFrameworkServiceDescriptor this[string key]
    {
      get => this._descriptors[key];
      set => this._descriptors[key] = value;
    }

    public ICollection<string> Keys => (ICollection<string>) this._descriptors.Keys;

    public ICollection<VssFrameworkServiceDescriptor> Values => (ICollection<VssFrameworkServiceDescriptor>) this._descriptors.Values;

    public int Count => this._descriptors.Count;

    bool ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>.IsReadOnly => ((ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>) this._descriptors).IsReadOnly;

    public void Add(string key, VssFrameworkServiceDescriptor value) => this._descriptors.Add(key, value);

    public void Clear() => this._descriptors.Clear();

    public bool ContainsKey(string key) => this._descriptors.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, VssFrameworkServiceDescriptor>> GetEnumerator() => (IEnumerator<KeyValuePair<string, VssFrameworkServiceDescriptor>>) this._descriptors.GetEnumerator();

    public bool Remove(string key) => this._descriptors.Remove(key);

    public bool TryGetValue(string key, out VssFrameworkServiceDescriptor value) => this._descriptors.TryGetValue(key, out value);

    void ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>.Add(
      KeyValuePair<string, VssFrameworkServiceDescriptor> item)
    {
      ((ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>) this._descriptors).Add(item);
    }

    bool ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>.Contains(
      KeyValuePair<string, VssFrameworkServiceDescriptor> item)
    {
      return this._descriptors.Contains<KeyValuePair<string, VssFrameworkServiceDescriptor>>(item);
    }

    void ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>.CopyTo(
      KeyValuePair<string, VssFrameworkServiceDescriptor>[] array,
      int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>) this._descriptors).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>.Remove(
      KeyValuePair<string, VssFrameworkServiceDescriptor> item)
    {
      return ((ICollection<KeyValuePair<string, VssFrameworkServiceDescriptor>>) this._descriptors).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._descriptors.GetEnumerator();
  }
}
