// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.JObjectAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class JObjectAccessor : 
    IReadOnlyObject,
    IReadOnlyDictionary<string, object>,
    IReadOnlyCollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly JObject m_jobject;

    public JObjectAccessor(JObject jobject) => this.m_jobject = jobject;

    public int Count => this.m_jobject.Count;

    public IEnumerable<string> Keys => (IEnumerable<string>) ((IDictionary<string, JToken>) this.m_jobject).Keys;

    public IEnumerable<object> Values => (IEnumerable<object>) this.m_jobject.Select<KeyValuePair<string, JToken>, JToken>((Func<KeyValuePair<string, JToken>, JToken>) (x => x.Value));

    public object this[string key] => (object) this.m_jobject[key];

    public bool ContainsKey(string key) => this.m_jobject.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.m_jobject.Select<KeyValuePair<string, JToken>, KeyValuePair<string, object>>((Func<KeyValuePair<string, JToken>, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x.Key, (object) x.Value))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_jobject.Select<KeyValuePair<string, JToken>, KeyValuePair<string, object>>((Func<KeyValuePair<string, JToken>, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x.Key, (object) x.Value))).GetEnumerator();

    public bool TryGetValue(string key, out object value)
    {
      JToken jtoken;
      if (this.m_jobject.TryGetValue(key, out jtoken))
      {
        value = (object) jtoken;
        return true;
      }
      value = (object) null;
      return false;
    }
  }
}
