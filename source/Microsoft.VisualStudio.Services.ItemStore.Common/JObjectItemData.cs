// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.JObjectItemData
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class JObjectItemData : IItemData
  {
    private readonly JObject data;

    public JObjectItemData()
      : this(new JObject())
    {
    }

    public JObjectItemData(JObject data) => this.data = data;

    public string StorageETag
    {
      get => this.data[nameof (StorageETag)] == null ? (string) null : this.data["StorageEtag"].ToString();
      set => this.data["StorageEtag"] = (JToken) value;
    }

    public IEnumerable<string> Keys => this.data.Properties().Select<JProperty, string>((Func<JProperty, string>) (p => p.Name));

    public string this[string key]
    {
      get => this.data[key] is JValue jvalue && jvalue.Value != null ? jvalue.Value.ToString() : (string) null;
      set => this.data[key] = (JToken) value;
    }

    public Microsoft.VisualStudio.Services.ItemStore.Common.Item GetItem(string key, int index = -1)
    {
      object obj = (object) this.data[key];
      switch (obj)
      {
        case null:
        case string _:
        case string[] _:
          return (Microsoft.VisualStudio.Services.ItemStore.Common.Item) null;
        default:
          JObject data = (JObject) null;
          if (obj is JObject)
          {
            if (index != -1 && index != 0)
              throw new ArgumentOutOfRangeException(string.Format("No item found at index {0}", (object) index));
            data = obj as JObject;
          }
          else if (obj is JArray && index >= 0)
            data = ((JArray) obj)[index] as JObject;
          return data == null ? (Microsoft.VisualStudio.Services.ItemStore.Common.Item) null : (Microsoft.VisualStudio.Services.ItemStore.Common.Item) new JObjectItem(data);
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item> GetItems(string key)
    {
      object data1 = (object) this.data[key];
      switch (data1)
      {
        case null:
        case string _:
          return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) null;
        case JObject _:
          return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) new JObjectItem[1]
          {
            new JObjectItem(data1 as JObject)
          };
        case JArray _:
          JArray jarray = data1 as JArray;
          List<Microsoft.VisualStudio.Services.ItemStore.Common.Item> items = new List<Microsoft.VisualStudio.Services.ItemStore.Common.Item>(jarray.Count);
          foreach (JToken data2 in jarray)
          {
            if (data2 is JValue)
              return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) null;
            items.Add((Microsoft.VisualStudio.Services.ItemStore.Common.Item) new JObjectItem(data2 as JObject));
          }
          return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) items;
        default:
          return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) null;
      }
    }

    public void SetItem(string key, Microsoft.VisualStudio.Services.ItemStore.Common.Item value)
    {
      if (value == null)
      {
        this.data[key] = (JToken) null;
      }
      else
      {
        if (!(value.Data is JObjectItemData data))
          throw new InvalidOperationException("Cannot mix ItemData types");
        this.data[key] = (JToken) data.data;
      }
    }

    public void SetItems(string key, IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item> items)
    {
      if (!items.All<Microsoft.VisualStudio.Services.ItemStore.Common.Item>((Func<Microsoft.VisualStudio.Services.ItemStore.Common.Item, bool>) (h => h.Data is JObjectItemData)))
        throw new InvalidOperationException("Cannot mix item ItemData types");
      this.data[key] = (JToken) new JArray((object[]) items.Select<Microsoft.VisualStudio.Services.ItemStore.Common.Item, JObject>((Func<Microsoft.VisualStudio.Services.ItemStore.Common.Item, JObject>) (h => ((JObjectItemData) h.Data).data)).ToArray<JObject>());
    }

    public IEnumerable<string> GetStrings(string key)
    {
      JToken jtoken = this.data[key];
      try
      {
        return jtoken is JArray ? (IEnumerable<string>) jtoken.ToObject<string[]>() : (IEnumerable<string>) (string[]) null;
      }
      catch (JsonReaderException ex)
      {
        return (IEnumerable<string>) null;
      }
    }

    public void SetMaybeStrings(string key, IEnumerable<string> values) => this.data[key] = values == null ? (JToken) null : (JToken) new JArray((object) values);

    public void SetStrings(string key, IEnumerable<string> values) => this.data[key] = (JToken) new JArray((object) values);

    public bool? GetBool(string key)
    {
      JToken jtoken = this.data[key];
      return !(jtoken is JValue) ? new bool?() : jtoken.ToObject<bool?>();
    }

    public JObject ToJson() => this.data;
  }
}
