// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.JsonSerializable
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents
{
  public abstract class JsonSerializable
  {
    internal JObject propertyBag;
    private const string POCOSerializationOnly = "POCOSerializationOnly";
    internal static bool JustPocoSerialization;

    static JsonSerializable()
    {
      int result;
      if (int.TryParse(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(nameof (POCOSerializationOnly))) ? "0" : Environment.GetEnvironmentVariable(nameof (POCOSerializationOnly)), out result) && result == 1)
        JsonSerializable.JustPocoSerialization = true;
      else
        JsonSerializable.JustPocoSerialization = false;
    }

    internal JsonSerializable() => this.propertyBag = new JObject();

    internal JsonSerializerSettings SerializerSettings { get; set; }

    public void SaveTo(Stream stream, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None) => this.SaveTo(stream, formattingPolicy, (JsonSerializerSettings) null);

    public void SaveTo(
      Stream stream,
      SerializationFormattingPolicy formattingPolicy,
      JsonSerializerSettings settings)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this.SerializerSettings = settings;
      JsonSerializer serializer = settings == null ? new JsonSerializer() : JsonSerializer.Create(settings);
      this.SaveTo((JsonWriter) new JsonTextWriter((TextWriter) new StreamWriter(stream)), serializer, formattingPolicy);
    }

    internal void SaveTo(
      JsonWriter writer,
      JsonSerializer serializer,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      writer.Formatting = formattingPolicy != SerializationFormattingPolicy.Indented ? Formatting.None : Formatting.Indented;
      this.OnSave();
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Document), this.GetType()) && !this.GetType().Equals(typeof (Document)) || CustomTypeExtensions.IsAssignableFrom(typeof (Attachment), this.GetType()) && !this.GetType().Equals(typeof (Attachment)))
        serializer.Serialize(writer, (object) this);
      else if (JsonSerializable.JustPocoSerialization)
        this.propertyBag.WriteTo(writer);
      else
        serializer.Serialize(writer, (object) this.propertyBag);
      writer.Flush();
    }

    internal void SaveTo(
      StringBuilder stringBuilder,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      if (stringBuilder == null)
        throw new ArgumentNullException(nameof (stringBuilder));
      this.SaveTo((JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(stringBuilder, (IFormatProvider) CultureInfo.CurrentCulture)), new JsonSerializer(), formattingPolicy);
    }

    public virtual void LoadFrom(JsonReader reader) => this.propertyBag = reader != null ? JObject.Load(reader) : throw new ArgumentNullException(nameof (reader));

    public virtual void LoadFrom(JsonReader reader, JsonSerializerSettings serializerSettings)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      Helpers.SetupJsonReader(reader, serializerSettings);
      this.propertyBag = JObject.Load(reader);
      this.SerializerSettings = serializerSettings;
    }

    public static T LoadFrom<T>(Stream stream) where T : JsonSerializable, new() => JsonSerializable.LoadFrom<T>(stream, (ITypeResolver<T>) null);

    internal static T LoadFrom<T>(
      Stream stream,
      ITypeResolver<T> typeResolver,
      JsonSerializerSettings settings = null)
      where T : JsonSerializable, new()
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      return JsonSerializable.LoadFrom<T>(new JsonTextReader((TextReader) new StreamReader(stream)), typeResolver, settings);
    }

    internal static T LoadFromWithResolver<T>(
      Stream stream,
      ITypeResolver<T> typeResolver,
      JsonSerializerSettings settings = null)
      where T : JsonSerializable
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (typeResolver == null)
        throw new ArgumentNullException(nameof (typeResolver));
      JsonTextReader jsonReader = new JsonTextReader((TextReader) new StreamReader(stream));
      return JsonSerializable.LoadFromWithResolver<T>(typeResolver, settings, jsonReader);
    }

    internal static T LoadFromWithResolver<T>(
      string serialized,
      ITypeResolver<T> typeResolver,
      JsonSerializerSettings settings = null)
      where T : JsonSerializable
    {
      if (serialized == null)
        throw new ArgumentNullException(nameof (serialized));
      if (typeResolver == null)
        throw new ArgumentNullException(nameof (typeResolver));
      JsonTextReader jsonReader = new JsonTextReader((TextReader) new StringReader(serialized));
      return JsonSerializable.LoadFromWithResolver<T>(typeResolver, settings, jsonReader);
    }

    internal static T LoadFrom<T>(
      string serialized,
      ITypeResolver<T> typeResolver,
      JsonSerializerSettings settings = null)
      where T : JsonSerializable, new()
    {
      if (serialized == null)
        throw new ArgumentNullException(nameof (serialized));
      return JsonSerializable.LoadFrom<T>(new JsonTextReader((TextReader) new StringReader(serialized)), typeResolver, settings);
    }

    public static T LoadFromWithConstructor<T>(Stream stream, Func<T> constructorFunction) => JsonSerializable.LoadFromWithConstructor<T>(stream, constructorFunction, (JsonSerializerSettings) null);

    public static T LoadFromWithConstructor<T>(
      Stream stream,
      Func<T> constructorFunction,
      JsonSerializerSettings settings)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (!CustomTypeExtensions.IsSubclassOf(typeof (T), typeof (JsonSerializable)))
        throw new ArgumentException("type is not serializable");
      T obj = constructorFunction();
      ((JsonSerializable) (object) obj).LoadFrom((JsonReader) new JsonTextReader((TextReader) new StreamReader(stream)), settings);
      return obj;
    }

    public override string ToString()
    {
      this.OnSave();
      return this.propertyBag.ToString();
    }

    internal virtual void Validate()
    {
    }

    internal T GetValue<T>(string propertyName)
    {
      if (this.propertyBag != null)
      {
        JToken jtoken = this.propertyBag[propertyName];
        if (jtoken != null)
        {
          if (typeof (T).IsEnum() && jtoken.Type == JTokenType.String)
            return jtoken.ToObject<T>(JsonSerializer.CreateDefault());
          return this.SerializerSettings != null ? jtoken.ToObject<T>(JsonSerializer.Create(this.SerializerSettings)) : jtoken.ToObject<T>();
        }
      }
      return default (T);
    }

    internal T GetValue<T>(string propertyName, T defaultValue)
    {
      if (this.propertyBag != null)
      {
        JToken jtoken = this.propertyBag[propertyName];
        if (jtoken != null)
        {
          if (typeof (T).IsEnum() && jtoken.Type == JTokenType.String)
            return jtoken.ToObject<T>(JsonSerializer.CreateDefault());
          return this.SerializerSettings != null ? jtoken.ToObject<T>(JsonSerializer.Create(this.SerializerSettings)) : jtoken.ToObject<T>();
        }
      }
      return defaultValue;
    }

    internal T GetValueByPath<T>(string[] fieldNames, T defaultValue)
    {
      if (fieldNames == null)
        throw new ArgumentNullException(nameof (fieldNames));
      if (fieldNames.Length == 0)
        throw new ArgumentException("fieldNames is empty.");
      if (this.propertyBag != null)
      {
        JToken jtoken = this.propertyBag[fieldNames[0]];
        for (int index = 1; index < fieldNames.Length && jtoken != null; ++index)
          jtoken = jtoken is JObject ? jtoken[(object) fieldNames[index]] : (JToken) null;
        if (jtoken != null)
        {
          if (typeof (T).IsEnum() && jtoken.Type == JTokenType.String)
            return jtoken.ToObject<T>(JsonSerializer.CreateDefault());
          return this.SerializerSettings != null ? jtoken.ToObject<T>(JsonSerializer.Create(this.SerializerSettings)) : jtoken.ToObject<T>();
        }
      }
      return defaultValue;
    }

    internal void SetValue(string name, object value)
    {
      if (this.propertyBag == null)
        this.propertyBag = new JObject();
      if (value != null)
        this.propertyBag[name] = JToken.FromObject(value);
      else
        this.propertyBag.Remove(name);
    }

    internal void SetValueByPath<T>(string[] fieldNames, T value)
    {
      if (fieldNames == null)
        throw new ArgumentNullException(nameof (fieldNames));
      if (fieldNames.Length == 0)
        throw new ArgumentException("fieldNames is empty.");
      if (this.propertyBag == null)
        this.propertyBag = new JObject();
      JToken propertyBag = (JToken) this.propertyBag;
      for (int index = 0; index < fieldNames.Length - 1; ++index)
      {
        if (propertyBag[(object) fieldNames[index]] == null)
          propertyBag[(object) fieldNames[index]] = (JToken) new JObject();
        propertyBag = propertyBag[(object) fieldNames[index]];
      }
      JObject jobject = propertyBag as JObject;
      if ((object) value == null && jobject != null)
        jobject.Remove(fieldNames[fieldNames.Length - 1]);
      else
        propertyBag[(object) fieldNames[fieldNames.Length - 1]] = (object) value == null ? (JToken) null : JToken.FromObject((object) value);
    }

    internal TSerializable GetObject<TSerializable>(string propertyName) where TSerializable : JsonSerializable, new()
    {
      if (this.propertyBag != null)
      {
        JToken o = this.propertyBag[propertyName];
        if (o != null && o.HasValues)
        {
          TSerializable serializable = new TSerializable();
          serializable.propertyBag = JObject.FromObject((object) o);
          return serializable;
        }
      }
      return default (TSerializable);
    }

    internal TSerializable GetObjectWithResolver<TSerializable>(
      string propertyName,
      ITypeResolver<TSerializable> typeResolver)
      where TSerializable : JsonSerializable
    {
      if (this.propertyBag != null)
      {
        JToken propertyBag = this.propertyBag[propertyName];
        if (propertyBag != null && propertyBag.HasValues)
          return propertyBag is JObject ? typeResolver.Resolve(propertyBag as JObject) : throw new ArgumentException(string.Format("Cannot resolve property type. The property {0} is not an object, it is a {1}.", (object) propertyName, (object) propertyBag.Type));
      }
      return default (TSerializable);
    }

    internal void SetObject<TSerializable>(string propertyName, TSerializable value) where TSerializable : JsonSerializable
    {
      if (this.propertyBag == null)
        this.propertyBag = new JObject();
      this.propertyBag[propertyName] = (object) value != null ? (JToken) value.propertyBag : (JToken) null;
    }

    internal Collection<TSerializable> GetObjectCollection<TSerializable>(
      string propertyName,
      Type resourceType = null,
      string ownerName = null,
      ITypeResolver<TSerializable> typeResolver = null)
      where TSerializable : JsonSerializable, new()
    {
      if (this.propertyBag != null)
      {
        JToken jtoken = this.propertyBag[propertyName];
        if (typeResolver == null)
          typeResolver = JsonSerializable.GetTypeResolver<TSerializable>();
        if (jtoken != null)
        {
          Collection<JObject> collection = jtoken.ToObject<Collection<JObject>>();
          Collection<TSerializable> objectCollection = new Collection<TSerializable>();
          foreach (JObject propertyBag in collection)
          {
            TSerializable serializable = typeResolver != null ? typeResolver.Resolve(propertyBag) : new TSerializable();
            serializable.propertyBag = propertyBag;
            if (PathsHelper.IsPublicResource(typeof (TSerializable)))
            {
              Resource resource = (object) serializable as Resource;
              resource.AltLink = PathsHelper.GeneratePathForNameBased(resourceType, ownerName, resource.Id);
            }
            objectCollection.Add(serializable);
          }
          return objectCollection;
        }
      }
      return (Collection<TSerializable>) null;
    }

    internal Collection<TSerializable> GetObjectCollectionWithResolver<TSerializable>(
      string propertyName,
      ITypeResolver<TSerializable> typeResolver)
      where TSerializable : JsonSerializable
    {
      if (this.propertyBag != null)
      {
        JToken jtoken = this.propertyBag[propertyName];
        if (jtoken != null)
        {
          Collection<JObject> collection = jtoken.ToObject<Collection<JObject>>();
          Collection<TSerializable> collectionWithResolver = new Collection<TSerializable>();
          foreach (JObject propertyBag in collection)
          {
            TSerializable serializable = typeResolver.Resolve(propertyBag);
            serializable.propertyBag = propertyBag;
            collectionWithResolver.Add(serializable);
          }
          return collectionWithResolver;
        }
      }
      return (Collection<TSerializable>) null;
    }

    internal void SetObjectCollection<TSerializable>(
      string propertyName,
      Collection<TSerializable> value)
      where TSerializable : JsonSerializable
    {
      if (this.propertyBag == null)
        this.propertyBag = new JObject();
      if (value == null)
        return;
      Collection<JObject> o = new Collection<JObject>();
      foreach (TSerializable serializable in value)
      {
        serializable.OnSave();
        o.Add(serializable.propertyBag ?? new JObject());
      }
      this.propertyBag[propertyName] = JToken.FromObject((object) o);
    }

    internal Dictionary<string, TSerializable> GetObjectDictionary<TSerializable>(
      string propertyName,
      ITypeResolver<TSerializable> typeResolver = null)
      where TSerializable : JsonSerializable, new()
    {
      if (this.propertyBag != null)
      {
        JToken jtoken = this.propertyBag[propertyName];
        if (typeResolver == null)
          typeResolver = JsonSerializable.GetTypeResolver<TSerializable>();
        if (jtoken != null)
        {
          Dictionary<string, JObject> dictionary = jtoken.ToObject<Dictionary<string, JObject>>();
          Dictionary<string, TSerializable> objectDictionary = new Dictionary<string, TSerializable>();
          foreach (KeyValuePair<string, JObject> keyValuePair in dictionary)
          {
            TSerializable serializable = typeResolver != null ? typeResolver.Resolve(keyValuePair.Value) : new TSerializable();
            serializable.propertyBag = keyValuePair.Value;
            objectDictionary.Add(keyValuePair.Key, serializable);
          }
          return objectDictionary;
        }
      }
      return (Dictionary<string, TSerializable>) null;
    }

    internal void SetObjectDictionary<TSerializable>(
      string propertyName,
      Dictionary<string, TSerializable> value)
      where TSerializable : JsonSerializable, new()
    {
      if (this.propertyBag == null)
        this.propertyBag = new JObject();
      if (value == null)
        return;
      Dictionary<string, JObject> o = new Dictionary<string, JObject>();
      foreach (KeyValuePair<string, TSerializable> keyValuePair in value)
      {
        keyValuePair.Value.OnSave();
        o.Add(keyValuePair.Key, keyValuePair.Value.propertyBag ?? new JObject());
      }
      this.propertyBag[propertyName] = JToken.FromObject((object) o);
    }

    internal virtual void OnSave()
    {
    }

    internal static ITypeResolver<TResource> GetTypeResolver<TResource>() where TResource : JsonSerializable, new()
    {
      ITypeResolver<TResource> typeResolver = (ITypeResolver<TResource>) null;
      if ((object) typeof (TResource) == (object) typeof (Offer))
        typeResolver = (ITypeResolver<TResource>) OfferTypeResolver.ResponseOfferTypeResolver;
      return typeResolver;
    }

    private static T LoadFrom<T>(
      JsonTextReader jsonReader,
      ITypeResolver<T> typeResolver,
      JsonSerializerSettings settings = null)
      where T : JsonSerializable, new()
    {
      T obj = new T();
      obj.LoadFrom((JsonReader) jsonReader, settings);
      return typeResolver != null ? typeResolver.Resolve(obj.propertyBag) : obj;
    }

    private static T LoadFromWithResolver<T>(
      ITypeResolver<T> typeResolver,
      JsonSerializerSettings settings,
      JsonTextReader jsonReader)
      where T : JsonSerializable
    {
      Helpers.SetupJsonReader((JsonReader) jsonReader, settings);
      JObject propertyBag = JObject.Load((JsonReader) jsonReader);
      return typeResolver.Resolve(propertyBag);
    }
  }
}
