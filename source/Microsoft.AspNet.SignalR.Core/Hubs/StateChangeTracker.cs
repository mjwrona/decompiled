// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.StateChangeTracker
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class StateChangeTracker
  {
    private readonly IDictionary<string, object> _values;
    private readonly IDictionary<string, object> _oldValues = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public StateChangeTracker() => this._values = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public StateChangeTracker(IDictionary<string, object> values) => this._values = values;

    public object this[string key]
    {
      get
      {
        object obj;
        this._values.TryGetValue(key, out obj);
        return DynamicDictionary.Wrap(obj);
      }
      set
      {
        if (!this._oldValues.ContainsKey(key))
        {
          object obj;
          this._values.TryGetValue(key, out obj);
          this._oldValues[key] = obj;
        }
        this._values[key] = value;
      }
    }

    public IDictionary<string, object> GetChanges()
    {
      Dictionary<string, object> dictionary = this._oldValues.Keys.Select(key => new
      {
        key = key,
        oldValue = this._oldValues[key]
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        newValue = this._values[_param1.key]
      }).Where(_param1 => !object.Equals(_param1.\u003C\u003Eh__TransparentIdentifier0.oldValue, _param1.newValue)).Select(_param1 => new
      {
        Key = _param1.\u003C\u003Eh__TransparentIdentifier0.key,
        Value = _param1.newValue
      }).ToDictionary(p => p.Key, p => p.Value);
      return dictionary.Count <= 0 ? (IDictionary<string, object>) null : (IDictionary<string, object>) dictionary;
    }
  }
}
