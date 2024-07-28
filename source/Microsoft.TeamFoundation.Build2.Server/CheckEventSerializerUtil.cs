// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CheckEventSerializerUtil
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class CheckEventSerializerUtil
  {
    private const int FullGzipHeaderLength = 10;
    private static readonly byte[] GzipHeader = new byte[2]
    {
      (byte) 31,
      (byte) 139
    };
    private static readonly Encoding s_UTF8NoBOM = (Encoding) new UTF8Encoding(false, true);
    private static readonly Lazy<JsonSerializerSettings> s_serializerSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new JsonSerializerSettings()
    {
      TypeNameHandling = TypeNameHandling.Objects
    }));
    private static readonly Lazy<JsonSerializerSettings> s_serializerSettingsWithSecurityFix = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new JsonSerializerSettings()
    {
      TypeNameHandling = TypeNameHandling.Objects,
      SerializationBinder = (ISerializationBinder) new CheckEventPayloadSerializationBinder(),
      ContractResolver = (IContractResolver) new DefaultContractResolver()
      {
        IgnoreSerializableInterface = true
      }
    }));
    private static readonly Lazy<JsonSerializer> s_serializer = new Lazy<JsonSerializer>((Func<JsonSerializer>) (() => JsonSerializer.Create(CheckEventSerializerUtil.s_serializerSettings.Value)));

    public static T Deserialize<T>(byte[] toDeserialize, bool securityFixEnabled)
    {
      Lazy<JsonSerializerSettings> lazy = securityFixEnabled ? CheckEventSerializerUtil.s_serializerSettingsWithSecurityFix : CheckEventSerializerUtil.s_serializerSettings;
      return CheckEventSerializerUtil.Deserialize<T>(toDeserialize, lazy.Value);
    }

    public static T Deserialize<T>(byte[] toDeserialize, JsonSerializerSettings settings)
    {
      if (toDeserialize == null || toDeserialize.Length == 0)
        return default (T);
      using (MemoryStream memoryStream = new MemoryStream(toDeserialize))
      {
        Stream stream = (Stream) memoryStream;
        if (CheckEventSerializerUtil.IsGZipStream(toDeserialize))
          stream = (Stream) new GZipStream((Stream) memoryStream, CompressionMode.Decompress);
        using (StreamReader reader1 = new StreamReader(stream, CheckEventSerializerUtil.s_UTF8NoBOM, true))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            return JsonSerializer.Create(settings).Deserialize<T>((JsonReader) reader2);
        }
      }
    }

    public static byte[] Serialize(object toSerialize, bool compress = true) => CheckEventSerializerUtil.Serialize(toSerialize, compress, CheckEventSerializerUtil.s_UTF8NoBOM);

    public static byte[] Serialize(object toSerialize, bool compress, Encoding encoding)
    {
      if (toSerialize == null)
        return (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Stream stream = (Stream) memoryStream;
        if (compress)
          stream = (Stream) new GZipStream((Stream) memoryStream, CompressionMode.Compress);
        using (StreamWriter streamWriter = new StreamWriter(stream, encoding ?? CheckEventSerializerUtil.s_UTF8NoBOM))
        {
          using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) streamWriter))
            CheckEventSerializerUtil.s_serializer.Value.Serialize((JsonWriter) jsonTextWriter, toSerialize);
        }
        return memoryStream.ToArray();
      }
    }

    private static bool IsGZipStream(byte[] data) => data != null && data.Length > 10 && (int) data[0] == (int) CheckEventSerializerUtil.GzipHeader[0] && (int) data[1] == (int) CheckEventSerializerUtil.GzipHeader[1];
  }
}
