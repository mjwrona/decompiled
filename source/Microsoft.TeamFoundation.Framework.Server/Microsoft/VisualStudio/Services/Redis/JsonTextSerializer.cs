// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.JsonTextSerializer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.IO.Compression;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class JsonTextSerializer : IValueSerializer
  {
    private readonly CompressionManager m_compressionManager;
    private readonly JsonSerializer m_jsonSerializer;
    private static readonly JsonSerializer s_jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new AllFieldsResolver()
    });

    public JsonTextSerializer(JsonSerializer jsonSerializer = null, bool enableCompression = false)
    {
      this.m_jsonSerializer = jsonSerializer ?? JsonTextSerializer.s_jsonSerializer;
      this.m_compressionManager = new CompressionManager(enableCompression);
    }

    byte[] IValueSerializer.Serialize<T>(T value)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (Stream outputStream = this.m_compressionManager.CreateOutputStream<T>((Stream) memoryStream))
        {
          using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) new StreamWriter(outputStream)))
            this.m_jsonSerializer.Serialize((JsonWriter) jsonTextWriter, (object) value);
        }
        return memoryStream.ToArray();
      }
    }

    T IValueSerializer.Deserialize<T>(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream(data))
      {
        using (Stream stream = this.IsGzipStream(data) ? (Stream) new GZipStream((Stream) memoryStream, CompressionMode.Decompress) : (Stream) memoryStream)
        {
          using (JsonTextReader reader = new JsonTextReader((TextReader) new StreamReader(stream)))
            return this.m_jsonSerializer.Deserialize<T>((JsonReader) reader);
        }
      }
    }

    private bool IsGzipStream(byte[] data) => data.Length >= 18 && data[0] == (byte) 31 && data[1] == (byte) 139 && data[2] == (byte) 8;
  }
}
