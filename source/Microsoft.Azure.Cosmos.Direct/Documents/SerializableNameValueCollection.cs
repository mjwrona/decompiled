// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SerializableNameValueCollection
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Azure.Documents
{
  internal sealed class SerializableNameValueCollection : JsonSerializable
  {
    private Lazy<NameValueCollection> lazyCollection;

    public SerializableNameValueCollection() => this.lazyCollection = new Lazy<NameValueCollection>(new Func<NameValueCollection>(this.Init));

    public SerializableNameValueCollection(NameValueCollection collection)
    {
      this.lazyCollection = new Lazy<NameValueCollection>(new Func<NameValueCollection>(this.Init));
      this.Collection.Add(collection);
    }

    [JsonIgnore]
    public NameValueCollection Collection => this.lazyCollection.Value;

    public static string SaveToString(
      SerializableNameValueCollection nameValueCollection)
    {
      if (nameValueCollection == null)
        return string.Empty;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        nameValueCollection.SaveTo((Stream) memoryStream);
        memoryStream.Position = 0L;
        using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
          return streamReader.ReadToEnd();
      }
    }

    public static SerializableNameValueCollection LoadFromString(string value)
    {
      if (string.IsNullOrEmpty(value))
        return new SerializableNameValueCollection();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream))
        {
          streamWriter.Write(value);
          streamWriter.Flush();
          memoryStream.Position = 0L;
          return JsonSerializable.LoadFrom<SerializableNameValueCollection>((Stream) memoryStream);
        }
      }
    }

    internal override void OnSave()
    {
      foreach (string name in (NameObjectCollectionBase) this.Collection)
        this.SetValue(name, (object) this.Collection[name]);
    }

    private NameValueCollection Init()
    {
      NameValueCollection nameValueCollection = new NameValueCollection();
      if (this.propertyBag != null)
      {
        foreach (KeyValuePair<string, JToken> keyValuePair in this.propertyBag)
        {
          if (keyValuePair.Value is JValue jvalue)
            nameValueCollection.Add(keyValuePair.Key, jvalue.ToString());
        }
      }
      return nameValueCollection;
    }

    public override bool Equals(object obj) => this.Equals(obj as SerializableNameValueCollection);

    public bool Equals(SerializableNameValueCollection collection)
    {
      if (collection == null)
        return false;
      return this == collection || this.IsEqual(collection);
    }

    private bool IsEqual(
      SerializableNameValueCollection serializableNameValueCollection)
    {
      if (this.Collection.Count != serializableNameValueCollection.Collection.Count)
        return false;
      foreach (string allKey in this.Collection.AllKeys)
      {
        if (this.Collection[allKey] != serializableNameValueCollection.Collection[allKey])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      foreach (string name in (NameObjectCollectionBase) this.Collection)
      {
        hashCode = hashCode * 397 ^ name.GetHashCode();
        hashCode = hashCode * 397 ^ (this.Collection.Get(name) != null ? this.Collection.Get(name).GetHashCode() : 0);
      }
      return hashCode;
    }
  }
}
