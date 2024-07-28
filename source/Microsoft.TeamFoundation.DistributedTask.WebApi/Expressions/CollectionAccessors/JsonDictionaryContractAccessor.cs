// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.JsonDictionaryContractAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class JsonDictionaryContractAccessor : 
    IReadOnlyObject,
    IReadOnlyDictionary<string, object>,
    IReadOnlyCollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private static Lazy<MethodInfo> s_getCountTemplate = new Lazy<MethodInfo>((Func<MethodInfo>) (() => typeof (JsonDictionaryContractAccessor).GetTypeInfo().GetMethod("GetCount", BindingFlags.Static | BindingFlags.NonPublic)));
    private static Lazy<MethodInfo> s_getKeysTemplate = new Lazy<MethodInfo>((Func<MethodInfo>) (() => typeof (JsonDictionaryContractAccessor).GetTypeInfo().GetMethod("GetKeys", BindingFlags.Static | BindingFlags.NonPublic)));
    private static Lazy<MethodInfo> s_tryGetValueTemplate = new Lazy<MethodInfo>((Func<MethodInfo>) (() => typeof (JsonDictionaryContractAccessor).GetTypeInfo().GetMethod("TryGetValue", BindingFlags.Static | BindingFlags.NonPublic)));
    private readonly JsonDictionaryContract m_contract;
    private readonly object m_obj;

    public JsonDictionaryContractAccessor(JsonDictionaryContract contract, object obj)
    {
      this.m_contract = contract;
      this.m_obj = obj;
    }

    public int Count => (int) JsonDictionaryContractAccessor.s_getCountTemplate.Value.MakeGenericMethod(this.m_contract.DictionaryValueType).Invoke((object) null, new object[1]
    {
      this.m_obj
    });

    public IEnumerable<string> Keys => JsonDictionaryContractAccessor.s_getKeysTemplate.Value.MakeGenericMethod(this.m_contract.DictionaryValueType).Invoke((object) null, new object[1]
    {
      this.m_obj
    }) as IEnumerable<string>;

    public IEnumerable<object> Values => this.Keys.Select<string, object>((Func<string, object>) (x => this[x]));

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

    public bool ContainsKey(string key) => this.TryGetValue(key, out object _);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.Keys.Select<string, KeyValuePair<string, object>>((Func<string, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x, this[x]))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Keys.Select<string, KeyValuePair<string, object>>((Func<string, KeyValuePair<string, object>>) (x => new KeyValuePair<string, object>(x, this[x]))).GetEnumerator();

    public bool TryGetValue(string key, out object value)
    {
      Tuple<bool, object> tuple = JsonDictionaryContractAccessor.s_tryGetValueTemplate.Value.MakeGenericMethod(this.m_contract.DictionaryValueType).Invoke((object) null, new object[2]
      {
        this.m_obj,
        (object) key
      }) as Tuple<bool, object>;
      value = tuple.Item2;
      return tuple.Item1;
    }

    private static int GetCount<TValue>(IDictionary<string, TValue> dictionary) => dictionary.Count;

    private static IEnumerable<string> GetKeys<TValue>(IDictionary<string, TValue> dictionary) => (IEnumerable<string>) dictionary.Keys;

    private static Tuple<bool, object> TryGetValue<TValue>(
      IDictionary<string, TValue> dictionary,
      string key)
    {
      TValue obj;
      return dictionary.TryGetValue(key, out obj) ? new Tuple<bool, object>(true, (object) obj) : new Tuple<bool, object>(false, (object) null);
    }
  }
}
