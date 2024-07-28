// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.JsonUtility
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class JsonUtility
  {
    private const int FullGzipHeaderLength = 10;
    private static readonly byte[] GzipHeader = new byte[2]
    {
      (byte) 31,
      (byte) 139
    };
    private static readonly Encoding s_UTF8NoBOM = (Encoding) new UTF8Encoding(false, true);
    private static readonly Lazy<JsonSerializerSettings> s_serializerSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new VssJsonMediaTypeFormatter().SerializerSettings));
    private static readonly Lazy<JsonSerializerSettings> s_indentSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() =>
    {
      JsonSerializerSettings serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;
      serializerSettings.Formatting = Formatting.Indented;
      return serializerSettings;
    }));
    private static readonly Lazy<JsonSerializer> s_defaultSerializer = new Lazy<JsonSerializer>((Func<JsonSerializer>) (() => JsonSerializer.Create(JsonUtility.s_serializerSettings.Value)));

    public static JsonSerializer CreateJsonSerializer() => JsonSerializer.Create(JsonUtility.s_serializerSettings.Value);

    public static T FromString<T>(string toDeserialize) => JsonUtility.FromString<T>(toDeserialize, JsonUtility.s_serializerSettings.Value);

    public static T FromString<T>(string toDeserialize, JsonSerializerSettings settings)
    {
      if (string.IsNullOrEmpty(toDeserialize))
        return default (T);
      using (StringReader reader1 = new StringReader(toDeserialize))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          return JsonSerializer.Create(settings).Deserialize<T>((JsonReader) reader2);
      }
    }

    public static void Populate(string toDeserialize, object target)
    {
      using (StringReader reader1 = new StringReader(toDeserialize))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          JsonUtility.s_defaultSerializer.Value.Populate((JsonReader) reader2, target);
      }
    }

    public static string ToString(object toSerialize) => JsonUtility.ToString(toSerialize, false);

    public static string ToString<T>(IList<T> toSerialize) => toSerialize == null || toSerialize.Count == 0 ? (string) null : JsonUtility.ToString((object) toSerialize, false);

    public static string ToString(object toSerialize, bool indent)
    {
      if (toSerialize == null)
        return (string) null;
      StringBuilder sb = new StringBuilder();
      using (StringWriter stringWriter = new StringWriter(sb))
      {
        using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) stringWriter))
        {
          if (indent)
            JsonSerializer.Create(JsonUtility.s_indentSettings.Value).Serialize((JsonWriter) jsonTextWriter, toSerialize);
          else
            JsonUtility.s_defaultSerializer.Value.Serialize((JsonWriter) jsonTextWriter, toSerialize);
        }
      }
      return sb.ToString();
    }

    public static T Deserialize<T>(Stream streamToRead) => JsonUtility.Deserialize<T>(streamToRead, false);

    public static T Deserialize<T>(Stream streamToRead, bool leaveOpen)
    {
      if (streamToRead == null)
        return default (T);
      using (StreamReader reader1 = new StreamReader(streamToRead, JsonUtility.s_UTF8NoBOM, true, 81920, leaveOpen))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          return JsonUtility.s_defaultSerializer.Value.Deserialize<T>((JsonReader) reader2);
      }
    }

    public static T Deserialize<T>(byte[] toDeserialize) => JsonUtility.Deserialize<T>(toDeserialize, JsonUtility.s_serializerSettings.Value);

    public static T Deserialize<T>(byte[] toDeserialize, JsonSerializerSettings settings)
    {
      if (toDeserialize == null || toDeserialize.Length == 0)
        return default (T);
      using (MemoryStream memoryStream = new MemoryStream(toDeserialize))
      {
        Stream stream = (Stream) memoryStream;
        if (JsonUtility.IsGZipStream(toDeserialize))
          stream = (Stream) new GZipStream((Stream) memoryStream, CompressionMode.Decompress);
        using (StreamReader reader1 = new StreamReader(stream, JsonUtility.s_UTF8NoBOM, true))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            return JsonSerializer.Create(settings).Deserialize<T>((JsonReader) reader2);
        }
      }
    }

    public static JToken Map(
      this JToken token,
      Dictionary<JTokenType, Func<JToken, JToken>> mapFuncs)
    {
      mapFuncs = mapFuncs ?? new Dictionary<JTokenType, Func<JToken, JToken>>();
      switch (token.Type)
      {
        case JTokenType.Object:
          JObject jobject = new JObject();
          foreach (JProperty child in token.Children<JProperty>())
          {
            JToken token1 = child.Value;
            if (token1.HasValues)
              token1 = token1.Map(mapFuncs);
            Func<JToken, JToken> func;
            if (mapFuncs.TryGetValue(token1.Type, out func))
              token1 = func(token1);
            jobject.Add(child.Name, token1);
          }
          return (JToken) jobject;
        case JTokenType.Array:
          JArray jarray = new JArray();
          foreach (JToken child in token.Children())
          {
            JToken token2 = child;
            if (token2.HasValues)
              token2 = token2.Map(mapFuncs);
            Func<JToken, JToken> func;
            if (mapFuncs.TryGetValue(token2.Type, out func))
              token2 = func(token2);
            jarray.Add(token2);
          }
          return (JToken) jarray;
        case JTokenType.String:
          Func<JToken, JToken> func1;
          return mapFuncs.TryGetValue(JTokenType.String, out func1) ? func1(token) : token;
        case JTokenType.Boolean:
        case JTokenType.Null:
        case JTokenType.Guid:
          return token;
        default:
          throw new NotSupportedException(WebApiResources.UnexpectedTokenType());
      }
    }

    public static byte[] Serialize(object toSerialize, bool compress = true) => JsonUtility.Serialize(toSerialize, compress, JsonUtility.s_UTF8NoBOM);

    public static byte[] Serialize(object toSerialize, bool compress, Encoding encoding)
    {
      if (toSerialize == null)
        return (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Stream stream = (Stream) memoryStream;
        if (compress)
          stream = (Stream) new GZipStream((Stream) memoryStream, CompressionMode.Compress);
        using (StreamWriter streamWriter = new StreamWriter(stream, encoding ?? JsonUtility.s_UTF8NoBOM))
        {
          using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) streamWriter))
            JsonUtility.s_defaultSerializer.Value.Serialize((JsonWriter) jsonTextWriter, toSerialize);
        }
        return memoryStream.ToArray();
      }
    }

    private static bool IsGZipStream(byte[] data) => data != null && data.Length > 10 && (int) data[0] == (int) JsonUtility.GzipHeader[0] && (int) data[1] == (int) JsonUtility.GzipHeader[1];
  }
}
