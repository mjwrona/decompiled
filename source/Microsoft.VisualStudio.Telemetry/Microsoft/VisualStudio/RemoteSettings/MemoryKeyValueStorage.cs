// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.MemoryKeyValueStorage
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class MemoryKeyValueStorage : ICollectionKeyValueStorage
  {
    private Dictionary<string, MemoryKeyValueStorage.KeyValueCollection> collections = new Dictionary<string, MemoryKeyValueStorage.KeyValueCollection>();

    public bool CollectionExists(string collectionPath)
    {
      collectionPath = collectionPath.NormalizePath();
      return this.collections.ContainsKey(collectionPath);
    }

    public bool PropertyExists(string collectionPath, string key)
    {
      collectionPath = collectionPath.NormalizePath();
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      return this.collections.TryGetValue(collectionPath, out keyValueCollection) && keyValueCollection.Properties.TryGetValue(key, out object _);
    }

    public bool DeleteCollection(string collectionPath) => throw new InvalidOperationException("MemoryKeyValueStorage does not support deleting collections");

    public bool DeleteProperty(string collectionPath, string propertyName) => throw new InvalidOperationException("MemoryKeyValueStorage does not support deleting properties");

    public IEnumerable<string> GetPropertyNames(string collectionPath)
    {
      collectionPath = collectionPath.NormalizePath();
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      return this.collections.TryGetValue(collectionPath, out keyValueCollection) ? (IEnumerable<string>) keyValueCollection.Properties.Keys.ToList<string>() : Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetSubCollectionNames(string collectionPath)
    {
      collectionPath = collectionPath.NormalizePath();
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      return this.collections.TryGetValue(collectionPath, out keyValueCollection) ? (IEnumerable<string>) keyValueCollection.SubCollections.ToList<string>() : Enumerable.Empty<string>();
    }

    public T GetValue<T>(string collectionPath, string key, T defaultValue)
    {
      T obj;
      this.TryGetValueInternal<T>(collectionPath, key, defaultValue, out obj);
      return obj;
    }

    public bool TryGetValue<T>(string collectionPath, string key, out T value) => this.TryGetValueInternal<T>(collectionPath, key, default (T), out value);

    public bool TryGetValueKind(string collectionPath, string key, out ValueKind kind)
    {
      collectionPath = collectionPath.NormalizePath();
      kind = ValueKind.Unknown;
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      object obj;
      if (!this.collections.TryGetValue(collectionPath, out keyValueCollection) || !keyValueCollection.Properties.TryGetValue(key, out obj))
        return false;
      System.Type type = obj.GetType();
      kind = type == typeof (double) || type == typeof (ulong) || type == typeof (long) ? ValueKind.QWord : (type == typeof (short) || type == typeof (ushort) || type == typeof (int) || type == typeof (uint) || type == typeof (float) || type == typeof (bool) ? ValueKind.DWord : (!(type == typeof (string)) ? ValueKind.Unknown : ValueKind.String));
      return true;
    }

    public void SetValue<T>(string collectionPath, string key, T value)
    {
      collectionPath = collectionPath.NormalizePath();
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      if (!this.collections.TryGetValue(collectionPath, out keyValueCollection))
      {
        keyValueCollection = new MemoryKeyValueStorage.KeyValueCollection();
        this.collections[collectionPath] = keyValueCollection;
      }
      this.AddToParentCollections(collectionPath);
      keyValueCollection.Properties[key] = (object) value;
    }

    private void AddToParentCollections(string collectionPath)
    {
      if (!(collectionPath != string.Empty))
        return;
      int length = collectionPath.LastIndexOf('\\');
      string str1;
      string str2;
      if (length != -1)
      {
        str1 = collectionPath.Substring(0, length);
        str2 = collectionPath.Substring(length + 1);
      }
      else
      {
        str1 = string.Empty;
        str2 = collectionPath;
      }
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      if (!this.collections.TryGetValue(str1, out keyValueCollection))
      {
        keyValueCollection = new MemoryKeyValueStorage.KeyValueCollection();
        this.collections[str1] = keyValueCollection;
      }
      keyValueCollection.SubCollections.Add(str2);
      this.AddToParentCollections(str1);
    }

    private bool TryGetValueInternal<T>(
      string collectionPath,
      string key,
      T defaultValue,
      out T value)
    {
      collectionPath = collectionPath.NormalizePath();
      MemoryKeyValueStorage.KeyValueCollection keyValueCollection;
      object originalValue;
      if (this.collections.TryGetValue(collectionPath, out keyValueCollection) && keyValueCollection.Properties.TryGetValue(key, out originalValue))
        return originalValue.TryConvertToType<T>(defaultValue, out value);
      value = defaultValue;
      return false;
    }

    private class KeyValueCollection
    {
      public KeyValueCollection()
      {
        this.SubCollections = new HashSet<string>();
        this.Properties = new Dictionary<string, object>();
      }

      public HashSet<string> SubCollections { get; set; }

      public Dictionary<string, object> Properties { get; set; }
    }
  }
}
