// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.TemplateTokenReadOnlyDictionary
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal sealed class TemplateTokenReadOnlyDictionary : 
    IReadOnlyObject,
    IReadOnlyDictionary<string, object>,
    IReadOnlyCollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly MappingToken m_mapping;
    private Dictionary<string, object> m_dictionary;

    internal TemplateTokenReadOnlyDictionary(MappingToken mapping) => this.m_mapping = mapping;

    public int Count
    {
      get
      {
        if (this.m_dictionary == null)
          this.Initialize();
        return this.m_dictionary.Count;
      }
    }

    public IEnumerable<string> Keys
    {
      get
      {
        if (this.m_dictionary == null)
          this.Initialize();
        return (IEnumerable<string>) this.m_dictionary.Keys;
      }
    }

    public IEnumerable<object> Values
    {
      get
      {
        if (this.m_dictionary == null)
          this.Initialize();
        return (IEnumerable<object>) this.m_dictionary.Values;
      }
    }

    public object this[string key]
    {
      get
      {
        if (this.m_dictionary == null)
          this.Initialize();
        return this.m_dictionary[key];
      }
    }

    public bool ContainsKey(string key)
    {
      if (this.m_dictionary == null)
        this.Initialize();
      return this.m_dictionary.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      if (this.m_dictionary == null)
        this.Initialize();
      return (IEnumerator<KeyValuePair<string, object>>) this.m_dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      if (this.m_dictionary == null)
        this.Initialize();
      return (IEnumerator) this.m_dictionary.GetEnumerator();
    }

    public bool TryGetValue(string key, out object value)
    {
      if (this.m_dictionary == null)
        this.Initialize();
      return this.m_dictionary.TryGetValue(key, out value);
    }

    private void Initialize()
    {
      this.m_dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in this.m_mapping)
      {
        if (keyValuePair.Key is LiteralToken key && !this.m_dictionary.ContainsKey(key.Value))
          this.m_dictionary.Add(key.Value, (object) keyValuePair.Value);
      }
    }
  }
}
