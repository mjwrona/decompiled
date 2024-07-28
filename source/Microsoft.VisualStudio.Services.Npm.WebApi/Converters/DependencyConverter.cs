// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.DependencyConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class DependencyConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => objectType == typeof (Dictionary<string, string>);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.StartArray)
      {
        string[] strArray = serializer.Deserialize<string[]>(reader);
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (string key in strArray)
          dictionary.Add(key, string.Empty);
        return (object) dictionary;
      }
      if (reader.TokenType == JsonToken.StartObject)
        return (object) this.DeserializeObject(reader, serializer);
      if (string.IsNullOrEmpty(reader.Value.ToString()))
        return (object) null;
      throw new JsonSerializationException("Unexpected Format.");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    private Dictionary<string, string> DeserializeObject(
      JsonReader reader,
      JsonSerializer serializer)
    {
      JObject jobject = serializer.Deserialize<JObject>(reader);
      Dictionary<string, string> dictionary1 = (Dictionary<string, string>) null;
      JsonException jsonException = (JsonException) null;
      try
      {
        dictionary1 = jobject.ToObject<Dictionary<string, string>>();
      }
      catch (JsonReaderException ex)
      {
        jsonException = (JsonException) ex;
      }
      if (dictionary1 == null)
      {
        try
        {
          Dictionary<string, DependencyConverter.LegacyDependencyVersionFormat> dictionary2 = jobject.ToObject<Dictionary<string, DependencyConverter.LegacyDependencyVersionFormat>>();
          dictionary1 = new Dictionary<string, string>();
          foreach (KeyValuePair<string, DependencyConverter.LegacyDependencyVersionFormat> keyValuePair in dictionary2)
            dictionary1.Add(keyValuePair.Key, keyValuePair.Value.Version ?? string.Empty);
        }
        catch (JsonReaderException ex)
        {
          jsonException = jsonException ?? (JsonException) ex;
        }
      }
      return dictionary1 != null ? dictionary1 : throw jsonException ?? (JsonException) new JsonSerializationException("Unexpected Format.");
    }

    private class LegacyDependencyVersionFormat
    {
      public string Version { get; set; }
    }
  }
}
