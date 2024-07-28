// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.Serializers
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public static class Serializers
  {
    public static object FromXmlString(string str, Type type, IEnumerable<Type> knownTypes = null)
    {
      using (Stream stream = (Stream) new MemoryStream())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        stream.Write(bytes, 0, bytes.Length);
        stream.Position = 0L;
        return Serializers.GetDataContractSerializerBasedOnKnownTypes(type, knownTypes).ReadObject(stream);
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Calling Dispose multiple times for MemoryStream object does not break anything")]
    public static string ToXmlString(object value, Type type, IEnumerable<Type> knownTypes = null)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamReader streamReader = new StreamReader((Stream) memoryStream, Encoding.UTF8))
        {
          Serializers.GetDataContractSerializerBasedOnKnownTypes(type, knownTypes).WriteObject((Stream) memoryStream, value);
          memoryStream.Flush();
          memoryStream.Position = 0L;
          return streamReader.ReadToEnd();
        }
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Calling Dispose multiple times for MemoryStream object does not break anything")]
    public static string ToJsonString(object value, Type type, IEnumerable<Type> knownTypes = null)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamReader streamReader = new StreamReader((Stream) memoryStream, Encoding.UTF8))
        {
          Serializers.GetDataContractJsonSerializerBasedOnKnownTypes(type, knownTypes).WriteObject((Stream) memoryStream, value);
          memoryStream.Flush();
          memoryStream.Position = 0L;
          return streamReader.ReadToEnd();
        }
      }
    }

    public static object FromJsonString(string str, Type type, IEnumerable<Type> knownTypes = null)
    {
      using (Stream stream = (Stream) new MemoryStream())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        stream.Write(bytes, 0, bytes.Length);
        stream.Position = 0L;
        return Serializers.GetDataContractJsonSerializerBasedOnKnownTypes(type, knownTypes).ReadObject(stream);
      }
    }

    public static byte[] ToByteArray<T>(this T value) => (object) value == null ? (byte[]) null : Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) value, Formatting.Indented));

    public static T FromByteArray<T>(byte[] array)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        TypeNameHandling = TypeNameHandling.None
      };
      string message = Encoding.UTF8.GetString(array);
      Tracer.TraceInfo(1080209, "Indexing Pipeline", "Indexer", message);
      return JsonConvert.DeserializeObject<T>(message, settings);
    }

    internal static DataContractSerializer GetDataContractSerializerBasedOnKnownTypes(
      Type type,
      IEnumerable<Type> knownTypes)
    {
      return !knownTypes.IsNullOrEmpty<Type>() ? new DataContractSerializer(type, knownTypes) : new DataContractSerializer(type);
    }

    internal static DataContractJsonSerializer GetDataContractJsonSerializerBasedOnKnownTypes(
      Type type,
      IEnumerable<Type> knownTypes)
    {
      return !knownTypes.IsNullOrEmpty<Type>() ? new DataContractJsonSerializer(type, knownTypes) : new DataContractJsonSerializer(type);
    }
  }
}
