// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.CompositeVariables
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class CompositeVariables : 
    IReadOnlyDictionary<string, string>,
    IReadOnlyCollection<KeyValuePair<string, string>>,
    IEnumerable<KeyValuePair<string, string>>,
    IEnumerable
  {
    private IDictionary<string, string> m_systemVariables;
    private IDictionary<string, string> m_userVariables;

    public CompositeVariables(
      IDictionary<string, string> systemVariables,
      IDictionary<string, string> userVariables)
    {
      this.m_systemVariables = systemVariables;
      this.m_userVariables = userVariables;
    }

    public string this[string key]
    {
      get
      {
        string str;
        return this.m_systemVariables.TryGetValue(key, out str) ? str : this.m_userVariables[key];
      }
    }

    public int Count => this.m_systemVariables.Count + this.m_userVariables.Count;

    public IEnumerable<string> Keys
    {
      get
      {
        foreach (string key in (IEnumerable<string>) this.m_systemVariables.Keys)
          yield return key;
        foreach (string key in (IEnumerable<string>) this.m_userVariables.Keys)
          yield return key;
      }
    }

    public IEnumerable<string> Values
    {
      get
      {
        foreach (string str in (IEnumerable<string>) this.m_systemVariables.Values)
          yield return str;
        foreach (string str in (IEnumerable<string>) this.m_userVariables.Values)
          yield return str;
      }
    }

    public bool ContainsKey(string key) => this.m_systemVariables.ContainsKey(key) || this.m_userVariables.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => (IEnumerator<KeyValuePair<string, string>>) new CompositeVariablesEnumerator(this.m_systemVariables.GetEnumerator(), this.m_userVariables.GetEnumerator());

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new CompositeVariablesEnumerator(this.m_systemVariables.GetEnumerator(), this.m_userVariables.GetEnumerator());

    public bool TryGetValue(string key, out string value) => this.m_systemVariables.TryGetValue(key, out value) || this.m_userVariables.TryGetValue(key, out value);
  }
}
