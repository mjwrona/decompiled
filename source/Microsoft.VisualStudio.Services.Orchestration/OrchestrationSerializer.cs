// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationSerializer
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class OrchestrationSerializer
  {
    private static readonly Encoding DefaultEncoding = (Encoding) new UTF8Encoding(false);
    private readonly JsonSerializer serializer;

    public OrchestrationSerializer()
      : this((JsonSerializerSettings) null)
    {
    }

    public OrchestrationSerializer(JsonSerializerSettings settings)
    {
      this.serializer = JsonSerializer.Create(settings);
      this.serializer.TypeNameHandling = TypeNameHandling.Objects;
    }

    public OrchestrationSerializer(JsonSerializer serializer) => this.serializer = serializer;

    public string Serialize(object value, Formatting formatting)
    {
      StringWriter stringWriter = new StringWriter(new StringBuilder(256), (IFormatProvider) CultureInfo.InvariantCulture);
      using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) stringWriter))
      {
        jsonTextWriter.Formatting = formatting;
        this.serializer.Serialize((JsonWriter) jsonTextWriter, value);
      }
      return stringWriter.ToString();
    }

    public string Serialize(object value) => this.Serialize(value, this.serializer.Formatting);

    public object Deserialize(string data, Type objectType)
    {
      if (data == null)
        return (object) data;
      using (StringReader reader = new StringReader(data))
        return this.serializer.Deserialize((JsonReader) new JsonTextReader((TextReader) reader), objectType);
    }

    public T Deserialize<T>(string data) => (T) this.Deserialize(data, typeof (T));

    public byte[] SerializeToBytes(object value)
    {
      MemoryStream memoryStream = new MemoryStream();
      using (memoryStream)
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, OrchestrationSerializer.DefaultEncoding))
        {
          using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) streamWriter))
            this.serializer.Serialize((JsonWriter) jsonTextWriter, value);
        }
      }
      return memoryStream.ToArray();
    }

    public object DeserializeFromBytes(byte[] buffer, Type objectType)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (objectType == (Type) null)
        throw new ArgumentNullException(nameof (objectType));
      using (MemoryStream memoryStream = new MemoryStream(buffer, false))
      {
        using (StreamReader reader1 = new StreamReader((Stream) memoryStream, OrchestrationSerializer.DefaultEncoding))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            return this.serializer.Deserialize((JsonReader) reader2, objectType);
        }
      }
    }

    public T DeserializeFromBytes<T>(byte[] buffer) => (T) this.DeserializeFromBytes(buffer, typeof (T));

    public object DeserializeFromCompressedBytes(byte[] buffer, Type objectType)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (objectType == (Type) null)
        throw new ArgumentNullException(nameof (objectType));
      using (MemoryStream memoryStream = new MemoryStream(buffer))
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress, false))
        {
          using (StreamReader reader1 = new StreamReader((Stream) gzipStream, OrchestrationSerializer.DefaultEncoding))
          {
            using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
              return this.serializer.Deserialize((JsonReader) reader2, objectType);
          }
        }
      }
    }

    public T DeserializeFromCompressedBytes<T>(byte[] buffer) => (T) this.DeserializeFromCompressedBytes(buffer, typeof (T));

    public T DeserializeFromBytes<T>(byte[] buffer, bool compressed) => !compressed ? (T) this.DeserializeFromBytes(buffer, typeof (T)) : (T) this.DeserializeFromCompressedBytes(buffer, typeof (T));

    public byte[] Compress(byte[] objBytes)
    {
      if (objBytes == null)
        return (byte[]) null;
      byte[] destinationArray = (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionLevel.Optimal, true))
          gzipStream.Write(objBytes, 0, objBytes.Length);
        destinationArray = new byte[memoryStream.Length];
        Array.Copy((Array) memoryStream.GetBuffer(), 0, (Array) destinationArray, 0, destinationArray.Length);
      }
      return destinationArray;
    }

    public byte[] Decompress(byte[] objectBytes)
    {
      if (objectBytes == null)
        return (byte[]) null;
      byte[] destinationArray = (byte[]) null;
      using (MemoryStream destination = new MemoryStream())
      {
        using (MemoryStream memoryStream = new MemoryStream(objectBytes))
        {
          using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress, false))
            gzipStream.CopyTo((Stream) destination);
        }
        destinationArray = new byte[destination.Length];
        Array.Copy((Array) destination.GetBuffer(), 0, (Array) destinationArray, 0, destinationArray.Length);
      }
      return destinationArray;
    }
  }
}
