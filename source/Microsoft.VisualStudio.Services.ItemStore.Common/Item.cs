// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.Item
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class Item : IEquatable<Item>
  {
    public Item()
      : this((IItemData) new JObjectItemData())
    {
    }

    public Item(IItemData data) => this.Data = data;

    public IEnumerable<string> Keys => this.Data.Keys;

    public IItemData Data { get; }

    [IndexerName("ItemIndexer")]
    public string this[string key]
    {
      get => this.Data[key];
      set => this.Data[key] = value;
    }

    public static T FromJson<T>(JObject data) where T : Item
    {
      T instance = (T) Activator.CreateInstance(typeof (T), (object) new JObjectItemData(data));
      string verificationError;
      return instance.Verify(out verificationError) ? instance : throw new SerializationException(verificationError);
    }

    public static T FromJson<T>(string content) where T : Item
    {
      using (StringReader reader1 = new StringReader(content))
      {
        using (JsonReader reader2 = (JsonReader) new JsonTextReader((TextReader) reader1))
        {
          reader2.DateParseHandling = DateParseHandling.None;
          return Item.FromJson<T>(JObject.Load(reader2));
        }
      }
    }

    public static T CloneWithEtag<T>(T item) where T : StoredItem
    {
      string storageEtag = item.StorageETag;
      item = Item.FromJson<T>(item.ToJson().ToString());
      item.StorageETag = storageEtag;
      return item;
    }

    public T Convert<T>() where T : Item
    {
      if (typeof (T).IsAssignableFrom(this.GetType()))
        return (T) this;
      T instance = (T) Activator.CreateInstance(typeof (T), (object) this.Data);
      string verificationError;
      return instance.Verify(out verificationError) ? instance : throw new InvalidCastException(verificationError);
    }

    public IEnumerable<string> GetStrings(string key) => this.Data.GetStrings(key);

    public void SetStrings(string key, IEnumerable<string> values) => this.Data.SetStrings(key, values);

    public Item GetItem(string key, int index = -1) => this.Data.GetItem(key, index);

    public IEnumerable<Item> GetItems(string key) => this.Data.GetItems(key);

    public void SetItem(string key, Item value) => this.Data.SetItem(key, value);

    public void SetItems(string key, IEnumerable<Item> items) => this.Data.SetItems(key, items);

    public bool Equals(Item other, IComparer<Item> comparer) => comparer.Compare(this, other) == 0;

    public JObject ToJson()
    {
      string verificationError;
      if (!this.Verify(out verificationError))
        throw new SerializationException(verificationError);
      return this.Data.ToJson();
    }

    public bool Verify(out string verificationError)
    {
      Lazy<StringBuilder> errorBuilder = new Lazy<StringBuilder>();
      int num = this.Verify(errorBuilder) ? 1 : 0;
      verificationError = errorBuilder.IsValueCreated ? errorBuilder.Value.ToString() : string.Empty;
      return num != 0;
    }

    protected virtual bool Verify(Lazy<StringBuilder> errorBuilder) => true;

    public bool Equals(Item compareToItem) => compareToItem != null && this.Keys.SequenceEqual<string>(compareToItem.Keys, (IEqualityComparer<string>) StringComparer.Ordinal) && this.Keys.Select<string, string>((Func<string, string>) (k => this[k])).SequenceEqual<string>(compareToItem.Keys.Select<string, string>((Func<string, string>) (k => compareToItem[k])), (IEqualityComparer<string>) StringComparer.Ordinal);

    public override bool Equals(object obj) => obj is Item compareToItem && this.Equals(compareToItem);

    public override int GetHashCode() => 0;
  }
}
