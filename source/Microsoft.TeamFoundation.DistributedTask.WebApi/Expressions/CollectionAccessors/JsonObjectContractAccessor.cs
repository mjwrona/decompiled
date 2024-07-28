// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.JsonObjectContractAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class JsonObjectContractAccessor : 
    IReadOnlyObject,
    IReadOnlyDictionary<string, object>,
    IReadOnlyCollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly JsonObjectContract m_contract;
    private readonly object m_obj;

    public JsonObjectContractAccessor(JsonObjectContract contract, object obj)
    {
      this.m_contract = contract;
      this.m_obj = obj;
    }

    public int Count => this.GetProperties().Count<JsonProperty>();

    public IEnumerable<string> Keys => this.GetProperties().Select<JsonProperty, string>((Func<JsonProperty, string>) (x => x.PropertyName));

    public IEnumerable<object> Values => this.GetProperties().Select<JsonProperty, object>((Func<JsonProperty, object>) (x => x.ValueProvider.GetValue(this.m_obj)));

    public object this[string key]
    {
      get
      {
        object obj;
        if (this.TryGetValue(key, out obj))
          return obj;
        throw new KeyNotFoundException(ExpressionResources.KeyNotFound((object) key));
      }
    }

    public bool ContainsKey(string key) => this.TryGetProperty(key, out JsonProperty _);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.Keys.Select<string, KeyValuePair<string, object>>((Func<string, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x, this[x]))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Keys.Select<string, KeyValuePair<string, object>>((Func<string, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x, this[x]))).GetEnumerator();

    public bool TryGetValue(string key, out object value)
    {
      JsonProperty property;
      if (this.TryGetProperty(key, out property))
      {
        value = property.ValueProvider.GetValue(this.m_obj);
        return true;
      }
      value = (object) null;
      return false;
    }

    private IEnumerable<JsonProperty> GetProperties() => this.m_contract.Properties.Where<JsonProperty>((Func<JsonProperty, bool>) (x => !x.Ignored));

    private bool TryGetProperty(string key, out JsonProperty property)
    {
      property = this.m_contract.Properties.GetClosestMatchProperty(key);
      if (property != null && !property.Ignored)
        return true;
      property = (JsonProperty) null;
      return false;
    }
  }
}
