// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.AmqpMap
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class AmqpMap : IEnumerable<KeyValuePair<MapKey, object>>, IEnumerable
  {
    private IDictionary<MapKey, object> value;

    public AmqpMap() => this.value = (IDictionary<MapKey, object>) new Dictionary<MapKey, object>();

    public AmqpMap(IDictionary<MapKey, object> value) => this.value = value;

    public AmqpMap(IDictionary value)
      : this()
    {
      foreach (DictionaryEntry dictionaryEntry in value)
        this.value.Add(new MapKey(dictionaryEntry.Key), dictionaryEntry.Value);
    }

    public int Count => this.value.Count;

    public int ValueSize => MapEncoding.GetValueSize(this);

    public object this[MapKey key]
    {
      get
      {
        object obj;
        return this.value.TryGetValue(key, out obj) ? obj : (object) null;
      }
      set => this.value[key] = value;
    }

    public bool TryGetValue<TValue>(MapKey key, out TValue value)
    {
      object obj1;
      if (this.value.TryGetValue(key, out obj1))
      {
        if (obj1 == null)
        {
          value = default (TValue);
          return true;
        }
        if (obj1 is TValue obj2)
        {
          value = obj2;
          return true;
        }
      }
      value = default (TValue);
      return false;
    }

    public bool TryRemoveValue<TValue>(MapKey key, out TValue value)
    {
      if (!this.TryGetValue<TValue>(key, out value))
        return false;
      this.value.Remove(key);
      return true;
    }

    public void Add(MapKey key, object value) => this.value.Add(key, value);

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('[');
      bool flag = true;
      foreach (KeyValuePair<MapKey, object> keyValuePair in (IEnumerable<KeyValuePair<MapKey, object>>) this.value)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(',');
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", new object[2]
        {
          (object) keyValuePair.Key,
          keyValuePair.Value
        });
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.value.GetEnumerator();

    IEnumerator<KeyValuePair<MapKey, object>> IEnumerable<KeyValuePair<MapKey, object>>.GetEnumerator() => this.value.GetEnumerator();
  }
}
