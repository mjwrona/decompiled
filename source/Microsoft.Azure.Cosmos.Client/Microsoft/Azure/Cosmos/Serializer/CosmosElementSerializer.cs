// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serializer.CosmosElementSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serializer
{
  internal static class CosmosElementSerializer
  {
    internal static MemoryStream ToStream(
      string containerRid,
      IEnumerable<CosmosElement> cosmosElements,
      ResourceType resourceType,
      CosmosSerializationFormatOptions cosmosSerializationOptions = null)
    {
      IJsonWriter jsonWriter1 = cosmosSerializationOptions == null ? JsonWriter.Create(JsonSerializationFormat.Text) : cosmosSerializationOptions.CreateCustomWriterCallback();
      jsonWriter1.WriteObjectStart();
      jsonWriter1.WriteFieldName("_rid");
      jsonWriter1.WriteStringValue(containerRid);
      string rootNodeName = CosmosElementSerializer.GetRootNodeName(resourceType);
      jsonWriter1.WriteFieldName(rootNodeName);
      int num = 0;
      jsonWriter1.WriteArrayStart();
      foreach (CosmosElement cosmosElement in cosmosElements)
      {
        ++num;
        IJsonWriter jsonWriter2 = jsonWriter1;
        cosmosElement.WriteTo(jsonWriter2);
      }
      jsonWriter1.WriteArrayEnd();
      jsonWriter1.WriteFieldName("_count");
      jsonWriter1.WriteNumber64Value((Number64) (long) num);
      jsonWriter1.WriteObjectEnd();
      return CosmosElementSerializer.GetMemoryStreamFromJsonWriter(jsonWriter1);
    }

    internal static IReadOnlyList<T> GetResources<T>(
      IReadOnlyList<CosmosElement> cosmosArray,
      CosmosSerializerCore serializerCore)
    {
      if (cosmosArray == null)
        return (IReadOnlyList<T>) new List<T>();
      return typeof (CosmosElement).IsAssignableFrom(typeof (T)) ? (IReadOnlyList<T>) cosmosArray.Cast<T>().ToList<T>() : (IReadOnlyList<T>) CosmosElementSerializer.GetResourcesHelper<T>(cosmosArray, serializerCore);
    }

    internal static T[] GetResourcesHelper<T>(
      IReadOnlyList<CosmosElement> cosmosElements,
      CosmosSerializerCore serializerCore,
      CosmosSerializationFormatOptions cosmosSerializationOptions = null)
    {
      using (MemoryStream memoryStream = CosmosElementSerializer.ElementsToMemoryStream(cosmosElements, cosmosSerializationOptions))
        return serializerCore.FromFeedStream<T>((Stream) memoryStream);
    }

    internal static MemoryStream ElementsToMemoryStream(
      IReadOnlyList<CosmosElement> cosmosElements,
      CosmosSerializationFormatOptions cosmosSerializationOptions = null)
    {
      IJsonWriter jsonWriter = cosmosSerializationOptions == null ? JsonWriter.Create(JsonSerializationFormat.Text) : cosmosSerializationOptions.CreateCustomWriterCallback();
      jsonWriter.WriteArrayStart();
      foreach (CosmosElement cosmosElement in (IEnumerable<CosmosElement>) cosmosElements)
        cosmosElement.WriteTo(jsonWriter);
      jsonWriter.WriteArrayEnd();
      return CosmosElementSerializer.GetMemoryStreamFromJsonWriter(jsonWriter);
    }

    private static MemoryStream GetMemoryStreamFromJsonWriter(IJsonWriter jsonWriter)
    {
      ReadOnlyMemory<byte> result = jsonWriter.GetResult();
      ArraySegment<byte> segment;
      if (!MemoryMarshal.TryGetArray<byte>(result, out segment))
        segment = new ArraySegment<byte>(result.ToArray());
      return new MemoryStream(segment.Array, segment.Offset, segment.Count, false, true);
    }

    private static string GetRootNodeName(ResourceType resourceType) => resourceType == ResourceType.Collection ? "DocumentCollections" : resourceType.ToResourceTypeString() + "s";
  }
}
