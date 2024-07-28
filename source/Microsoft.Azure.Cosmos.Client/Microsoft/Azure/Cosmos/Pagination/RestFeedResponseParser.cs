// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.RestFeedResponseParser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Serializer;
using Microsoft.Azure.Documents;
using System;
using System.IO;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal static class RestFeedResponseParser
  {
    public static CosmosArray ParseRestFeedResponse(
      Stream stream,
      JsonSerializationFormatOptions jsonSerializationFormatOptions)
    {
      return RestFeedResponseParser.ParseRestFeedResponse(stream, ResourceType.Document, jsonSerializationFormatOptions);
    }

    public static CosmosArray ParseRestFeedResponse(
      Stream stream,
      ResourceType resourceType,
      JsonSerializationFormatOptions jsonSerializationFormatOptions)
    {
      return RestFeedResponseParser.GetResourceArrayFromNavigator(RestFeedResponseParser.CreateNavigatorFromContent(RestFeedResponseParser.StreamToBytes(stream), jsonSerializationFormatOptions), RestFeedResponseParser.ResourceTypeToArrayKeyName(resourceType));
    }

    private static ReadOnlyMemory<byte> StreamToBytes(Stream stream)
    {
      if (!(stream is MemoryStream destination))
      {
        destination = new MemoryStream();
        stream.CopyTo((Stream) destination);
      }
      if (!destination.CanRead)
        throw new InvalidDataException("Stream can not be read");
      ArraySegment<byte> buffer;
      return !destination.TryGetBuffer(out buffer) ? (ReadOnlyMemory<byte>) destination.ToArray() : (ReadOnlyMemory<byte>) buffer;
    }

    private static IJsonNavigator CreateNavigatorFromContent(
      ReadOnlyMemory<byte> content,
      JsonSerializationFormatOptions jsonSerializationFormatOptions)
    {
      IJsonNavigator navigatorFromContent;
      switch (jsonSerializationFormatOptions)
      {
        case null:
          navigatorFromContent = JsonNavigator.Create(content);
          break;
        case JsonSerializationFormatOptions.CustomJsonSerializationFormatOptions serializationFormatOptions:
          navigatorFromContent = serializationFormatOptions.createNavigator(content);
          if (navigatorFromContent == null)
            throw new InvalidOperationException("The CosmosSerializationOptions did not return a JSON navigator.");
          break;
        case JsonSerializationFormatOptions.NativelySupportedJsonSerializationFormatOptions _:
          navigatorFromContent = JsonNavigator.Create(content);
          break;
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unknown {0} type: {1}", (object) "JsonSerializationFormatOptions", (object) jsonSerializationFormatOptions.GetType()));
      }
      return navigatorFromContent;
    }

    private static string ResourceTypeToArrayKeyName(ResourceType resourceType)
    {
      switch (resourceType)
      {
        case ResourceType.Database:
          return "Databases";
        case ResourceType.Collection:
          return "DocumentCollections";
        case ResourceType.Document:
          return "Documents";
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", (object) nameof (resourceType), (object) resourceType));
      }
    }

    private static CosmosArray GetResourceArrayFromNavigator(
      IJsonNavigator jsonNavigator,
      string arrayKeyName)
    {
      ObjectProperty objectProperty;
      if (!jsonNavigator.TryGetObjectProperty(jsonNavigator.GetRootNode(), arrayKeyName, out objectProperty))
        throw new InvalidOperationException("Response Body Contract was violated. FeedResponse did not have property: " + arrayKeyName);
      if (CosmosElement.Dispatch(jsonNavigator, objectProperty.ValueNode) is CosmosArray arrayFromNavigator)
        return arrayFromNavigator;
      throw new InvalidOperationException("FeedResponse did not have an array of : " + arrayKeyName);
    }
  }
}
