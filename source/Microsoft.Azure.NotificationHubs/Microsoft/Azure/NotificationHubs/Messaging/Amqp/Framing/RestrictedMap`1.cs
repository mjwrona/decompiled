// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.RestrictedMap`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal abstract class RestrictedMap<TKey> : RestrictedMap
  {
    public static implicit operator AmqpMap(RestrictedMap<TKey> restrictedMap) => restrictedMap?.InnerMap;

    public object this[TKey key]
    {
      get => this.InnerMap[new MapKey((object) key)];
      set => this.InnerMap[new MapKey((object) key)] = value;
    }

    public object this[MapKey key]
    {
      get => this.InnerMap[key];
      set => this.InnerMap[key] = value;
    }

    public bool TryGetValue<TValue>(TKey key, out TValue value) => this.InnerMap.TryGetValue<TValue>(new MapKey((object) key), out value);

    public bool TryGetValue<TValue>(MapKey key, out TValue value) => this.InnerMap.TryGetValue<TValue>(key, out value);

    public bool TryRemoveValue<TValue>(TKey key, out TValue value) => this.InnerMap.TryRemoveValue<TValue>(new MapKey((object) key), out value);

    public void Add(TKey key, object value) => this.InnerMap.Add(new MapKey((object) key), value);

    public void Add(MapKey key, object value) => this.InnerMap.Add(key, value);

    public void Merge(RestrictedMap<TKey> map)
    {
      foreach (KeyValuePair<MapKey, object> keyValuePair in (IEnumerable<KeyValuePair<MapKey, object>>) map)
        this[keyValuePair.Key] = keyValuePair.Value;
    }
  }
}
