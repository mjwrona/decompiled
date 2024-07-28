// Decompiled with JetBrains decompiler
// Type: Nest.FuzzinessFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class FuzzinessFormatter : IJsonFormatter<Fuzziness>, IJsonFormatter
  {
    private static readonly byte[] AutoBytes = JsonWriter.GetEncodedPropertyNameWithoutQuotation("AUTO");

    public Fuzziness Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.Number:
          ArraySegment<byte> arraySegment1 = reader.ReadNumberSegment();
          int readCount1;
          return arraySegment1.IsDouble() ? Fuzziness.Ratio(NumberConverter.ReadDouble(arraySegment1.Array, arraySegment1.Offset, out readCount1)) : Fuzziness.EditDistance(NumberConverter.ReadInt32(arraySegment1.Array, arraySegment1.Offset, out readCount1));
        case JsonToken.String:
          ArraySegment<byte> arraySegment2 = reader.ReadStringSegmentUnsafe();
          if (arraySegment2.EqualsBytes(FuzzinessFormatter.AutoBytes))
            return Fuzziness.Auto;
          int num1 = -1;
          int num2 = -1;
          for (int length = FuzzinessFormatter.AutoBytes.Length; length < arraySegment2.Count; ++length)
          {
            if (arraySegment2.Array[arraySegment2.Offset + length] == (byte) 58)
              num1 = arraySegment2.Offset + length;
            else if (arraySegment2.Array[arraySegment2.Offset + length] == (byte) 44)
            {
              num2 = arraySegment2.Offset + length;
              break;
            }
          }
          int readCount2;
          return Fuzziness.AutoLength(NumberConverter.ReadInt32(arraySegment2.Array, num1 + 1, out readCount2), NumberConverter.ReadInt32(arraySegment2.Array, num2 + 1, out readCount2));
        default:
          reader.ReadNextBlock();
          return (Fuzziness) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Fuzziness value,
      IJsonFormatterResolver formatterResolver)
    {
      formatterResolver.GetFormatter<IFuzziness>().Serialize(ref writer, (IFuzziness) value, formatterResolver);
    }
  }
}
