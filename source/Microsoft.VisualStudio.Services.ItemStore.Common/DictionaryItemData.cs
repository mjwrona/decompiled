// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.DictionaryItemData
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class DictionaryItemData : IItemData
  {
    private readonly IDictionary<string, object> data;

    public DictionaryItemData()
      : this((IDictionary<string, object>) new Dictionary<string, object>())
    {
    }

    public DictionaryItemData(IDictionary<string, object> data) => this.data = data;

    public IEnumerable<string> Keys => (IEnumerable<string>) this.data.Keys;

    public string this[string key]
    {
      get
      {
        object obj;
        return !this.data.TryGetValue(key, out obj) ? (string) null : obj as string;
      }
      set => this.data[key] = (object) value;
    }

    public Microsoft.VisualStudio.Services.ItemStore.Common.Item GetItem(string key, int index = -1)
    {
      object obj;
      if (this.data.TryGetValue(key, out obj))
      {
        switch (obj)
        {
          case string _:
          case string[] _:
            break;
          default:
            IDictionary<string, object> data = (IDictionary<string, object>) null;
            if (obj is IDictionary<string, object> dictionary)
            {
              if (index != -1 && index != 0)
                throw new IndexOutOfRangeException(string.Format("No item found at index {0}", (object) index));
              data = dictionary;
            }
            else if (obj is Array && index >= 0 && obj is IDictionary<string, object>[] dictionaryArray)
              data = dictionaryArray[index];
            if (data == null)
            {
              IReadOnlyDictionary<string, object> source = (IReadOnlyDictionary<string, object>) null;
              if (obj is IReadOnlyDictionary<string, object>)
              {
                if (index != -1 && index != 0)
                  throw new IndexOutOfRangeException(string.Format("No item found at index {0}", (object) index));
                source = obj as IReadOnlyDictionary<string, object>;
              }
              else if (obj is Array && index >= 0 && obj is IReadOnlyDictionary<string, object>[] readOnlyDictionaryArray)
                source = readOnlyDictionaryArray[index];
              if (source != null)
                data = (IDictionary<string, object>) source.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
            }
            return data == null ? (Microsoft.VisualStudio.Services.ItemStore.Common.Item) null : (Microsoft.VisualStudio.Services.ItemStore.Common.Item) new DictionaryItem(data);
        }
      }
      return (Microsoft.VisualStudio.Services.ItemStore.Common.Item) null;
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item> GetItems(string key)
    {
      object obj;
      if (this.data.TryGetValue(key, out obj))
      {
        switch (obj)
        {
          case string _:
            break;
          case IDictionary<string, object> data1:
            return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) new DictionaryItem[1]
            {
              new DictionaryItem(data1)
            };
          case IReadOnlyDictionary<string, object> data2:
            return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) new DictionaryItem[1]
            {
              new DictionaryItem(data2)
            };
          case Array array:
            if (array.Length == 0)
              return Enumerable.Empty<Microsoft.VisualStudio.Services.ItemStore.Common.Item>();
            switch (obj)
            {
              case IDictionary<string, object>[] source1:
                return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) ((IEnumerable<IDictionary<string, object>>) source1).Select<IDictionary<string, object>, DictionaryItem>((Func<IDictionary<string, object>, DictionaryItem>) (element => new DictionaryItem(element)));
              case IReadOnlyDictionary<string, object>[] source2:
                return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) ((IEnumerable<IReadOnlyDictionary<string, object>>) source2).Select<IReadOnlyDictionary<string, object>, DictionaryItem>((Func<IReadOnlyDictionary<string, object>, DictionaryItem>) (element => new DictionaryItem(element)));
              default:
                return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) null;
            }
          default:
            return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) null;
        }
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item>) null;
    }

    public void SetItem(string key, Microsoft.VisualStudio.Services.ItemStore.Common.Item value)
    {
      if (!(value.Data is DictionaryItemData data))
        throw new InvalidOperationException("Cannot mix ItemData types");
      this.data[key] = (object) data.data;
    }

    public void SetItems(string key, IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item> items)
    {
      if (!items.All<Microsoft.VisualStudio.Services.ItemStore.Common.Item>((Func<Microsoft.VisualStudio.Services.ItemStore.Common.Item, bool>) (h => h.Data is DictionaryItemData)))
        throw new InvalidOperationException("Cannot mix ItemData types");
      this.data[key] = (object) items.Select<Microsoft.VisualStudio.Services.ItemStore.Common.Item, IDictionary<string, object>>((Func<Microsoft.VisualStudio.Services.ItemStore.Common.Item, IDictionary<string, object>>) (h => ((DictionaryItemData) h.Data).data)).ToArray<IDictionary<string, object>>();
    }

    public IEnumerable<string> GetStrings(string key)
    {
      object obj;
      return !this.data.TryGetValue(key, out obj) ? (IEnumerable<string>) null : (IEnumerable<string>) (obj as string[]);
    }

    public void SetStrings(string key, IEnumerable<string> values) => this.data[key] = (object) values.ToArray<string>();

    public void SetMaybeStrings(string key, IEnumerable<string> values) => this.data[key] = values == null ? (object) (string[]) null : (object) values.ToArray<string>();

    public bool? GetBool(string key)
    {
      string str = this[key];
      return new bool?(str != null && bool.Parse(str));
    }

    public JObject ToJson() => (JObject) JToken.FromObject((object) this.data);
  }
}
