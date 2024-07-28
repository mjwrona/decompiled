// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.UserVariables
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal class UserVariables : 
    IDictionary<string, string>,
    ICollection<KeyValuePair<string, string>>,
    IEnumerable<KeyValuePair<string, string>>,
    IEnumerable
  {
    private UserVariablesInternal m_current = new UserVariablesInternal((UserVariablesInternal) null);

    public string this[string key]
    {
      get
      {
        string str;
        if (this.TryGetValue(key, out str))
          return str;
        throw new KeyNotFoundException(YamlStrings.KeyNotFound((object) key));
      }
      set
      {
        int num = this.TryGetValue(key, out string _) ? 1 : 0;
        this.m_current.Dictionary[key] = value;
        if (num != 0)
          return;
        ++this.m_current.Count;
      }
    }

    public int Count => this.m_current.Count;

    public bool IsReadOnly => false;

    public ICollection<string> Keys => (ICollection<string>) this.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key)).ToList<string>();

    public ICollection<string> Values => (ICollection<string>) this.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value)).ToList<string>();

    public void Add(string key, string value)
    {
      if (this.TryGetValue(key, out string _))
        throw new ArgumentException(YamlStrings.KeyAlreadyDefined((object) key));
      this.m_current.Dictionary[key] = value;
      ++this.m_current.Count;
    }

    public void Add(KeyValuePair<string, string> item) => this.Add(item.Key, item.Value);

    public void Clear() => throw new NotSupportedException();

    public bool Contains(KeyValuePair<string, string> item) => throw new NotSupportedException();

    public bool ContainsKey(string key) => this.TryGetValue(key, out string _);

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
      foreach (KeyValuePair<string, string> keyValuePair in this)
        array[arrayIndex++] = keyValuePair;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => (IEnumerator<KeyValuePair<string, string>>) new UserVariablesEnumerator(this.m_current);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new UserVariablesEnumerator(this.m_current);

    public bool Remove(string key) => throw new NotSupportedException();

    public bool Remove(KeyValuePair<string, string> item) => throw new NotSupportedException();

    public bool TryGetValue(string key, out string value)
    {
      for (UserVariablesInternal variablesInternal = this.m_current; variablesInternal != null; variablesInternal = variablesInternal.Parent)
      {
        if (variablesInternal.Dictionary.TryGetValue(key, out value))
          return true;
      }
      value = (string) null;
      return false;
    }

    internal void PushScope() => this.m_current = new UserVariablesInternal(this.m_current);

    internal void PopScope() => this.m_current = this.m_current.Parent;
  }
}
