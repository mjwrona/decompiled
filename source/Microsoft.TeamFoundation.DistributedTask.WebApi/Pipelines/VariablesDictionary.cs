// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.VariablesDictionary
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VariablesDictionary : 
    IDictionary<string, VariableValue>,
    ICollection<KeyValuePair<string, VariableValue>>,
    IEnumerable<KeyValuePair<string, VariableValue>>,
    IEnumerable,
    IDictionary<string, string>,
    ICollection<KeyValuePair<string, string>>,
    IEnumerable<KeyValuePair<string, string>>
  {
    private readonly HashSet<string> m_secretsAccessed = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private IDictionary<string, VariableValue> m_variables = (IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public VariablesDictionary() => this.m_variables = (IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public VariablesDictionary(VariablesDictionary copyFrom)
      : this((IDictionary<string, VariableValue>) copyFrom, false)
    {
    }

    public VariablesDictionary(IDictionary<string, string> copyFrom)
      : this(copyFrom != null ? (IDictionary<string, VariableValue>) copyFrom.ToDictionary<KeyValuePair<string, string>, string, VariableValue>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, VariableValue>) (x => new VariableValue()
      {
        Value = x.Value
      }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, VariableValue>) null, false)
    {
    }

    public VariablesDictionary(IDictionary<string, VariableValue> copyFrom)
      : this(copyFrom, false)
    {
    }

    private VariablesDictionary(IDictionary<string, VariableValue> copyFrom, bool readOnly)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, VariableValue>>(copyFrom, nameof (copyFrom));
      if (readOnly)
        this.m_variables = (IDictionary<string, VariableValue>) new ReadOnlyDictionary<string, VariableValue>(copyFrom);
      else
        this.m_variables = (IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>(copyFrom, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> SecretsAccessed => this.m_secretsAccessed;

    public VariableValue this[string key]
    {
      get
      {
        VariableValue variableValue;
        if (!this.m_variables.TryGetValue(key, out variableValue))
          throw new KeyNotFoundException(key);
        if (variableValue.IsSecret)
          this.m_secretsAccessed.Add(key);
        return variableValue;
      }
      set => this.m_variables[key] = value;
    }

    public ICollection<string> Keys => this.m_variables.Keys;

    public ICollection<VariableValue> Values => this.m_variables.Values;

    public int Count => this.m_variables.Count;

    public bool IsReadOnly => this.m_variables.IsReadOnly;

    public void Add(string key, VariableValue value) => this.m_variables.Add(key, value);

    public void Add(KeyValuePair<string, VariableValue> item) => this.m_variables.Add(item);

    public VariablesDictionary AsReadOnly() => this.m_variables.IsReadOnly ? this : new VariablesDictionary(this.m_variables, true);

    public void Clear() => this.m_variables.Clear();

    public bool Contains(KeyValuePair<string, VariableValue> item) => this.m_variables.Contains(item);

    public bool ContainsKey(string key) => this.m_variables.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, VariableValue>[] array, int arrayIndex) => this.m_variables.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, VariableValue>> GetEnumerator() => this.m_variables.GetEnumerator();

    public bool Remove(string key) => this.m_variables.Remove(key);

    public bool Remove(KeyValuePair<string, VariableValue> item) => ((ICollection<KeyValuePair<string, VariableValue>>) this.m_variables).Remove(item);

    public bool TryGetValue(string key, out VariableValue value)
    {
      if (!this.m_variables.TryGetValue(key, out value))
        return false;
      if (value.IsSecret)
        this.m_secretsAccessed.Add(key);
      return true;
    }

    ICollection<string> IDictionary<string, string>.Keys => this.m_variables.Keys;

    ICollection<string> IDictionary<string, string>.Values => (ICollection<string>) this.m_variables.Select<KeyValuePair<string, VariableValue>, string>((Func<KeyValuePair<string, VariableValue>, string>) (x => x.Value?.Value)).ToArray<string>();

    int ICollection<KeyValuePair<string, string>>.Count => this.m_variables.Count;

    bool ICollection<KeyValuePair<string, string>>.IsReadOnly => this.m_variables.IsReadOnly;

    string IDictionary<string, string>.this[string key]
    {
      get
      {
        VariableValue variableValue;
        if (!this.m_variables.TryGetValue(key, out variableValue))
          throw new KeyNotFoundException(key);
        if (variableValue.IsSecret)
          this.m_secretsAccessed.Add(key);
        return variableValue.Value;
      }
      set
      {
        VariableValue variableValue;
        if (!this.m_variables.TryGetValue(key, out variableValue))
          this.m_variables.Add(key, (VariableValue) value);
        else
          variableValue.Value = value;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_variables.GetEnumerator();

    IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
    {
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) this.m_variables)
        yield return new KeyValuePair<string, string>(variable.Key, variable.Value?.Value);
    }

    bool IDictionary<string, string>.TryGetValue(string key, out string value)
    {
      VariableValue variableValue;
      if (this.m_variables.TryGetValue(key, out variableValue))
      {
        if (variableValue.IsSecret)
          this.m_secretsAccessed.Add(key);
        value = variableValue.Value;
        return true;
      }
      value = (string) null;
      return false;
    }

    bool IDictionary<string, string>.ContainsKey(string key) => this.m_variables.ContainsKey(key);

    void IDictionary<string, string>.Add(string key, string value) => this.m_variables.Add(key, (VariableValue) value);

    bool IDictionary<string, string>.Remove(string key) => this.m_variables.Remove(key);

    void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) => this.m_variables.Add(new KeyValuePair<string, VariableValue>(item.Key, (VariableValue) item.Value));

    void ICollection<KeyValuePair<string, string>>.Clear() => this.m_variables.Clear();

    bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => this.m_variables.Contains(new KeyValuePair<string, VariableValue>(item.Key, (VariableValue) item.Value));

    void ICollection<KeyValuePair<string, string>>.CopyTo(
      KeyValuePair<string, string>[] array,
      int arrayIndex)
    {
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) this.m_variables)
        array[arrayIndex++] = new KeyValuePair<string, string>(variable.Key, variable.Value?.Value);
    }

    bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) => ((ICollection<KeyValuePair<string, VariableValue>>) this.m_variables).Remove(new KeyValuePair<string, VariableValue>(item.Key, (VariableValue) item.Value));
  }
}
