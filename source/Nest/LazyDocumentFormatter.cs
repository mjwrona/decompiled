// Decompiled with JetBrains decompiler
// Type: Nest.LazyDocumentFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class LazyDocumentFormatter : IJsonFormatter<LazyDocument>, IJsonFormatter
  {
    internal static void WriteUnindented(ref JsonReader reader, ref JsonWriter writer)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          writer.WriteBeginObject();
          int count1 = 0;
          while (reader.ReadIsInObject(ref count1))
          {
            if (count1 != 1)
              writer.WriteRaw((byte) 44);
            writer.WritePropertyName(reader.ReadPropertyName());
            LazyDocumentFormatter.WriteUnindented(ref reader, ref writer);
          }
          writer.WriteEndObject();
          break;
        case JsonToken.BeginArray:
          writer.WriteBeginArray();
          int count2 = 0;
          while (reader.ReadIsInArray(ref count2))
          {
            if (count2 != 1)
              writer.WriteRaw((byte) 44);
            LazyDocumentFormatter.WriteUnindented(ref reader, ref writer);
          }
          writer.WriteEndArray();
          break;
        case JsonToken.Number:
          ArraySegment<byte> arraySegment = reader.ReadNumberSegment();
          for (int index = 0; index < arraySegment.Count; ++index)
            writer.WriteRawUnsafe(arraySegment.Array[index + arraySegment.Offset]);
          break;
        case JsonToken.String:
          string str = reader.ReadString();
          writer.WriteString(str);
          break;
        case JsonToken.True:
        case JsonToken.False:
          bool flag = reader.ReadBoolean();
          writer.WriteBoolean(flag);
          break;
        case JsonToken.Null:
          reader.ReadIsNull();
          writer.WriteNull();
          break;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      LazyDocument value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        JsonReader reader = new JsonReader(value.Bytes);
        LazyDocumentFormatter.WriteUnindented(ref reader, ref writer);
      }
    }

    public LazyDocument Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
        return (LazyDocument) null;
      ArraySegment<byte> src = reader.ReadNextBlockSegment();
      return new LazyDocument(BinaryUtil.ToArray(ref src), formatterResolver);
    }
  }
}
