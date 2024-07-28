// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.ReadOnlyDictionaryOfStringObjectAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class ReadOnlyDictionaryOfStringObjectAccessor : 
    IReadOnlyObject,
    IReadOnlyDictionary<string, object>,
    IReadOnlyCollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly IReadOnlyDictionary<string, object> m_dictionary;

    public ReadOnlyDictionaryOfStringObjectAccessor(IReadOnlyDictionary<string, object> dictionary) => this.m_dictionary = dictionary;

    public int Count => this.m_dictionary.Count;

    public IEnumerable<string> Keys => this.m_dictionary.Keys;

    public IEnumerable<object> Values => this.m_dictionary.Values;

    public object this[string key] => this.m_dictionary[key];

    public bool ContainsKey(string key) => this.m_dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.m_dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_dictionary.GetEnumerator();

    public bool TryGetValue(string key, out object value) => this.m_dictionary.TryGetValue(key, out value);
  }
}
