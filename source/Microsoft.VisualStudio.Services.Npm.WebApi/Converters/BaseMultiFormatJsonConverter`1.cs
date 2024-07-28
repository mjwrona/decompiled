// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.BaseMultiFormatJsonConverter`1
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public abstract class BaseMultiFormatJsonConverter<T> : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.String)
        return (object) this.ParseStringValue(reader.Value as string);
      if (reader.TokenType == JsonToken.StartObject)
        return (object) this.ParseProperties(this.GetObjectProperties(reader, serializer));
      if (reader.TokenType == JsonToken.StartArray)
        return (object) this.ParseArray(this.GetArray(reader, objectType, existingValue, serializer));
      throw new InvalidPackageJsonException("Unexpected Format.");
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (T);

    protected void WritePropertyIfNotEmpty(JsonWriter writer, string property, string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      writer.WritePropertyName(property);
      writer.WriteValue(value);
    }

    protected List<object> GetArray(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      List<object> array = new List<object>();
      while (reader.Read() && reader.TokenType != JsonToken.EndArray)
        array.Add(this.ParseToken(reader, objectType, existingValue, serializer));
      return array;
    }

    protected Dictionary<string, string> GetObjectProperties(
      JsonReader reader,
      JsonSerializer serializer)
    {
      JObject jobject = serializer.Deserialize<JObject>(reader);
      Dictionary<string, object> source = (Dictionary<string, object>) null;
      JsonException jsonException = (JsonException) null;
      try
      {
        source = jobject.ToObject<Dictionary<string, object>>();
      }
      catch (JsonReaderException ex)
      {
        jsonException = (JsonException) ex;
      }
      if (source == null)
        throw jsonException ?? (JsonException) new JsonSerializationException("Unexpected Format.");
      return source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (x => x.Value is string)).ToDictionary<KeyValuePair<string, object>, string, string>((Func<KeyValuePair<string, object>, string>) (x => x.Key), (Func<KeyValuePair<string, object>, string>) (x => (string) x.Value));
    }

    protected virtual object ParseToken(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return reader.Value;
    }

    protected abstract T ParseArray(List<object> tokens);

    protected abstract T ParseStringValue(string value);

    protected abstract T ParseProperties(Dictionary<string, string> properties);
  }
}
