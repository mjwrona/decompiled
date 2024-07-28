// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosFeedResponseSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


#nullable enable
namespace Microsoft.Azure.Cosmos
{
  internal static class CosmosFeedResponseSerializer
  {
    private const byte ArrayStart = 91;
    private const byte ArrayEnd = 93;

    internal static IReadOnlyCollection<T> FromFeedResponseStream<T>(
      CosmosSerializerCore serializerCore,
      Stream streamWithServiceEnvelope)
    {
      if (streamWithServiceEnvelope == null)
        return (IReadOnlyCollection<T>) new List<T>();
      using (streamWithServiceEnvelope)
      {
        using (MemoryStream withoutServiceEnvelope = CosmosFeedResponseSerializer.GetStreamWithoutServiceEnvelope(streamWithServiceEnvelope))
          return (IReadOnlyCollection<T>) serializerCore.FromFeedStream<T>((Stream) withoutServiceEnvelope);
      }
    }

    internal static MemoryStream GetStreamWithoutServiceEnvelope(Stream streamWithServiceEnvelope)
    {
      using (streamWithServiceEnvelope)
      {
        if (!(streamWithServiceEnvelope is MemoryStream destination))
        {
          destination = new MemoryStream();
          streamWithServiceEnvelope.CopyTo((Stream) destination);
          destination.Position = 0L;
        }
        ArraySegment<byte> buffer;
        ReadOnlyMemory<byte> memoryByte = !destination.TryGetBuffer(out buffer) ? (ReadOnlyMemory<byte>) destination.ToArray() : (ReadOnlyMemory<byte>) buffer;
        int arrayStartPosition = CosmosFeedResponseSerializer.GetArrayStartPosition(memoryByte);
        int arrayEndPosition = CosmosFeedResponseSerializer.GetArrayEndPosition(memoryByte);
        ReadOnlyMemory<byte> memory = memoryByte.Slice(arrayStartPosition, arrayEndPosition - arrayStartPosition + 1);
        ArraySegment<byte> segment;
        if (!MemoryMarshal.TryGetArray<byte>(memory, out segment))
          segment = new ArraySegment<byte>(memory.ToArray());
        return new MemoryStream(segment.Array, segment.Offset, segment.Count, false, true);
      }
    }

    private static int GetArrayStartPosition(ReadOnlyMemory<byte> memoryByte)
    {
      ReadOnlySpan<byte> span = memoryByte.Span;
      int num = span.IndexOf<byte>((byte) 91);
      return num >= 0 ? num : throw new InvalidDataException("Could not find the start of the json array in the stream: " + Encoding.UTF8.GetString(span));
    }

    private static int GetArrayEndPosition(ReadOnlyMemory<byte> memoryByte)
    {
      ReadOnlySpan<byte> span = memoryByte.Span;
      int num = span.LastIndexOf<byte>((byte) 93);
      return num >= 0 ? num : throw new InvalidDataException("Could not find the end of the json array in the stream: " + Encoding.UTF8.GetString(span));
    }
  }
}
