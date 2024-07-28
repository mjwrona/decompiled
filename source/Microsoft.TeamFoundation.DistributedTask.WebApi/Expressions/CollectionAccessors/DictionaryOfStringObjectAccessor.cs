// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.DictionaryOfStringObjectAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class DictionaryOfStringObjectAccessor : 
    IReadOnlyObject,
    IReadOnlyDictionary<string, object>,
    IReadOnlyCollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly IDictionary<string, object> m_dictionary;

    public DictionaryOfStringObjectAccessor(IDictionary<string, object> dictionary) => this.m_dictionary = dictionary;

    public int Count => this.m_dictionary.Count;

    public IEnumerable<string> Keys => (IEnumerable<string>) this.m_dictionary.Keys;

    public IEnumerable<object> Values => (IEnumerable<object>) this.m_dictionary.Values;

    public object this[string key] => this.m_dictionary[key];

    public bool ContainsKey(string key) => this.m_dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.m_dictionary.Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x.Key, x.Value))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_dictionary.Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x.Key, x.Value))).GetEnumerator();

    public bool TryGetValue(string key, out object value) => this.m_dictionary.TryGetValue(key, out value);
  }
}
