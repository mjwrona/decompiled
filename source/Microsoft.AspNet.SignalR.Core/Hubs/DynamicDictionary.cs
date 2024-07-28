// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.DynamicDictionary
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class DynamicDictionary : 
    DynamicObject,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly IDictionary<string, object> _obj;

    public DynamicDictionary(IDictionary<string, object> obj) => this._obj = obj;

    public object this[string key]
    {
      get
      {
        object obj;
        this._obj.TryGetValue(key, out obj);
        return DynamicDictionary.Wrap(obj);
      }
      set => this._obj[key] = DynamicDictionary.Unwrap(value);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = this[binder.Name];
      return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      this[binder.Name] = value;
      return true;
    }

    public static object Wrap(object value) => value is IDictionary<string, object> dictionary ? (object) new DynamicDictionary(dictionary) : value;

    public static object Unwrap(object value) => value is DynamicDictionary dynamicDictionary ? (object) dynamicDictionary._obj : value;

    public void Add(string key, object value) => this._obj.Add(key, value);

    public bool ContainsKey(string key) => this._obj.ContainsKey(key);

    public ICollection<string> Keys => this._obj.Keys;

    public bool Remove(string key) => this._obj.Remove(key);

    public bool TryGetValue(string key, out object value) => this._obj.TryGetValue(key, out value);

    public ICollection<object> Values => this._obj.Values;

    public void Add(KeyValuePair<string, object> item) => this._obj.Add(item);

    public void Clear() => this._obj.Clear();

    public bool Contains(KeyValuePair<string, object> item) => this._obj.Contains(item);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => this._obj.CopyTo(array, arrayIndex);

    public int Count => this._obj.Count;

    public bool IsReadOnly => this._obj.IsReadOnly;

    public bool Remove(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>) this._obj).Remove(item);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this._obj.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
