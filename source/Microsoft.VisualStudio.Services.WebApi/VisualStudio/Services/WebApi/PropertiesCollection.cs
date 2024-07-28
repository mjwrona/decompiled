// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.PropertiesCollection
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [CollectionDataContract(Name = "Properties", ItemName = "Property", KeyName = "Key", ValueName = "Value")]
  [JsonDictionary(ItemConverterType = typeof (PropertiesCollection.PropertiesCollectionItemConverter))]
  public sealed class PropertiesCollection : 
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    ICollection
  {
    private Dictionary<string, object> m_innerDictionary;

    public PropertiesCollection()
    {
      this.m_innerDictionary = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.PropertyName);
      this.ValidateNewValues = true;
    }

    public PropertiesCollection(IDictionary<string, object> source)
      : this(source, true)
    {
    }

    internal PropertiesCollection(IDictionary<string, object> source, bool validateExisting)
    {
      if (validateExisting)
        PropertyValidation.ValidateDictionary(source);
      this.m_innerDictionary = new Dictionary<string, object>(source, (IEqualityComparer<string>) VssStringComparer.PropertyName);
      this.ValidateNewValues = true;
    }

    internal bool ValidateNewValues { get; set; }

    public int Count => this.m_innerDictionary.Count;

    public object this[string key]
    {
      get => this.m_innerDictionary[key];
      set
      {
        if (this.ValidateNewValues)
        {
          PropertyValidation.ValidatePropertyName(key);
          PropertyValidation.ValidatePropertyValue(key, value);
        }
        this.m_innerDictionary[key] = value;
      }
    }

    public Dictionary<string, object>.KeyCollection Keys => this.m_innerDictionary.Keys;

    public Dictionary<string, object>.ValueCollection Values => this.m_innerDictionary.Values;

    public void Add(string key, object value)
    {
      if (this.ValidateNewValues)
      {
        PropertyValidation.ValidatePropertyName(key);
        PropertyValidation.ValidatePropertyValue(key, value);
      }
      this.m_innerDictionary.Add(key, value);
    }

    public void Clear() => this.m_innerDictionary.Clear();

    public bool ContainsKey(string key) => this.m_innerDictionary.ContainsKey(key);

    public bool ContainsValue(object value) => this.m_innerDictionary.ContainsValue(value);

    public bool Remove(string key) => this.m_innerDictionary.Remove(key);

    public T GetValue<T>(string key, T defaultValue)
    {
      T obj;
      if (!this.TryGetValue<T>(key, out obj))
        obj = defaultValue;
      return obj;
    }

    public bool TryGetValue(string key, out object value) => this.m_innerDictionary.TryGetValue(key, out value);

    public bool TryGetValue<T>(string key, out T value) => this.TryGetValidatedValue<T>(key, out value);

    public override bool Equals(object otherObj)
    {
      if (this == otherObj)
        return true;
      if (!(otherObj is PropertiesCollection propertiesCollection) || this.Count != propertiesCollection.Count)
        return false;
      foreach (string key in this.Keys)
      {
        object objA;
        if (!propertiesCollection.TryGetValue(key, out objA) || !object.Equals(objA, this[key]))
          return false;
      }
      return true;
    }

    public override int GetHashCode() => base.GetHashCode();

    void ICollection.CopyTo(Array array, int index) => ((ICollection) this.m_innerDictionary).CopyTo(array, index);

    bool ICollection.IsSynchronized => ((ICollection) this.m_innerDictionary).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection) this.m_innerDictionary).SyncRoot;

    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> keyValuePair)
    {
      if (this.ValidateNewValues)
      {
        PropertyValidation.ValidatePropertyName(keyValuePair.Key);
        PropertyValidation.ValidatePropertyValue(keyValuePair.Key, keyValuePair.Value);
      }
      ((ICollection<KeyValuePair<string, object>>) this.m_innerDictionary).Add(keyValuePair);
    }

    bool ICollection<KeyValuePair<string, object>>.Contains(
      KeyValuePair<string, object> keyValuePair)
    {
      return ((ICollection<KeyValuePair<string, object>>) this.m_innerDictionary).Contains(keyValuePair);
    }

    void ICollection<KeyValuePair<string, object>>.CopyTo(
      KeyValuePair<string, object>[] array,
      int index)
    {
      ((ICollection<KeyValuePair<string, object>>) this.m_innerDictionary).CopyTo(array, index);
    }

    bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> keyValuePair) => ((ICollection<KeyValuePair<string, object>>) this.m_innerDictionary).Remove(keyValuePair);

    ICollection<string> IDictionary<string, object>.Keys => ((IDictionary<string, object>) this.m_innerDictionary).Keys;

    ICollection<object> IDictionary<string, object>.Values => ((IDictionary<string, object>) this.m_innerDictionary).Values;

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => ((IEnumerable<KeyValuePair<string, object>>) this.m_innerDictionary).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.m_innerDictionary).GetEnumerator();

    internal class PropertiesCollectionItemConverter : JsonConverter
    {
      private const string TypePropertyName = "$type";
      private const string ValuePropertyName = "$value";

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        Type type = value.GetType();
        if (type.GetTypeInfo().IsEnum)
        {
          value = (object) ((Enum) value).ToString("D");
          type = typeof (string);
        }
        PropertyValidation.ValidatePropertyValue("being serialized", value);
        writer.WriteStartObject();
        writer.WritePropertyName("$type");
        string fullName = type.FullName;
        if (!PropertyValidation.IsValidTypeString(fullName))
          throw new PropertyTypeNotSupportedException("$type", type);
        writer.WriteValue(fullName);
        writer.WritePropertyName("$value");
        writer.WriteValue(value);
        writer.WriteEndObject();
      }

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        if (reader.TokenType == JsonToken.StartObject)
        {
          JObject source = serializer.Deserialize<JObject>(reader);
          JToken jtoken1;
          JToken jtoken2;
          if (!source.TryGetValue("$type", out jtoken1) || !source.TryGetValue("$value", out jtoken2))
          {
            IEnumerator<JToken> enumerator = source.Values().GetEnumerator();
            jtoken1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException(WebApiResources.DeserializationCorrupt());
            jtoken2 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException(WebApiResources.DeserializationCorrupt());
          }
          Type result;
          if (!PropertyValidation.TryGetValidType(jtoken1.ToObject<string>(), out result))
            throw new InvalidOperationException(WebApiResources.DeserializationCorrupt());
          return jtoken2.ToObject(result);
        }
        if (reader.TokenType == JsonToken.Boolean || reader.TokenType == JsonToken.Bytes || reader.TokenType == JsonToken.Date || reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.String)
          return serializer.Deserialize(reader);
        if (reader.TokenType == JsonToken.Null)
          return (object) null;
        throw new InvalidOperationException(WebApiResources.DeserializationCorrupt());
      }

      public override bool CanConvert(Type objectType) => true;
    }
  }
}
