// Decompiled with JetBrains decompiler
// Type: Nest.RangeQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class RangeQueryFormatter : IJsonFormatter<IRangeQuery>, IJsonFormatter
  {
    private static readonly AutomataDictionary RangeFields = new AutomataDictionary()
    {
      {
        "format",
        0
      },
      {
        "time_zone",
        1
      },
      {
        "gt",
        2
      },
      {
        "gte",
        3
      },
      {
        "lte",
        4
      },
      {
        "lt",
        5
      },
      {
        "from",
        6
      },
      {
        "to",
        7
      },
      {
        "include_lower",
        8
      },
      {
        "include_upper",
        9
      }
    };

    public IRangeQuery Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IRangeQuery) null;
      ArraySegment<byte> arraySegment1 = reader.ReadNextBlockSegment();
      Elasticsearch.Net.Utf8Json.JsonReader reader1 = new Elasticsearch.Net.Utf8Json.JsonReader(arraySegment1.Array, arraySegment1.Offset);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      int count1 = 0;
      while (reader1.ReadIsInObject(ref count1))
      {
        reader1.ReadPropertyNameSegmentRaw();
        int count2 = 0;
        while (reader1.ReadIsInObject(ref count2))
        {
          ArraySegment<byte> bytes = reader1.ReadPropertyNameSegmentRaw();
          int num;
          if (RangeQueryFormatter.RangeFields.TryGetValue(bytes, out num))
          {
            switch (num)
            {
              case 0:
              case 1:
                flag2 = true;
                break;
              case 2:
              case 3:
              case 4:
              case 5:
              case 6:
              case 7:
                switch (reader1.GetCurrentJsonToken())
                {
                  case JsonToken.Number:
                    if (!flag3)
                    {
                      ArraySegment<byte> arraySegment2 = reader1.ReadNumberSegment();
                      if (arraySegment2.IsDouble())
                      {
                        flag3 = true;
                        break;
                      }
                      flag1 = true;
                      break;
                    }
                    break;
                  case JsonToken.String:
                    if (!flag2)
                    {
                      ArraySegment<byte> arraySegment3 = reader1.ReadStringSegmentUnsafe();
                      flag2 = arraySegment3.IsDateTime(formatterResolver, out DateTime _) || arraySegment3.ContainsDateMathSeparator() && DateMath.IsValidDateMathString(arraySegment3.Utf8String());
                      break;
                    }
                    break;
                  case JsonToken.Null:
                    reader1.ReadIsNull();
                    break;
                }
                break;
              case 8:
              case 9:
                reader1.ReadBoolean();
                break;
            }
          }
          else
            reader1.ReadNextBlock();
          if (flag2 | flag3)
            break;
        }
        if (flag2 | flag3)
          break;
      }
      reader1 = new Elasticsearch.Net.Utf8Json.JsonReader(arraySegment1.Array, arraySegment1.Offset);
      if (flag2)
        return RangeQueryFormatter.Deserialize<IDateRangeQuery>(ref reader1, formatterResolver);
      if (flag3)
        return RangeQueryFormatter.Deserialize<INumericRangeQuery>(ref reader1, formatterResolver);
      return flag1 ? RangeQueryFormatter.Deserialize<ILongRangeQuery>(ref reader1, formatterResolver) : RangeQueryFormatter.Deserialize<ITermRangeQuery>(ref reader1, formatterResolver);
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IRangeQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      switch (value)
      {
        case null:
          writer.WriteNull();
          break;
        case IDateRangeQuery dateRangeQuery:
          RangeQueryFormatter.Serialize<IDateRangeQuery>(ref writer, dateRangeQuery, formatterResolver);
          break;
        case INumericRangeQuery numericRangeQuery:
          RangeQueryFormatter.Serialize<INumericRangeQuery>(ref writer, numericRangeQuery, formatterResolver);
          break;
        case ILongRangeQuery longRangeQuery:
          RangeQueryFormatter.Serialize<ILongRangeQuery>(ref writer, longRangeQuery, formatterResolver);
          break;
        case ITermRangeQuery termRangeQuery:
          RangeQueryFormatter.Serialize<ITermRangeQuery>(ref writer, termRangeQuery, formatterResolver);
          break;
        default:
          DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IRangeQuery>().Serialize(ref writer, value, formatterResolver);
          break;
      }
    }

    private static void Serialize<TQuery>(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      TQuery value,
      IJsonFormatterResolver formatterResolver)
      where TQuery : IRangeQuery
    {
      formatterResolver.GetFormatter<TQuery>().Serialize(ref writer, value, formatterResolver);
    }

    private static IRangeQuery Deserialize<TQuery>(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TQuery : IRangeQuery
    {
      return (IRangeQuery) formatterResolver.GetFormatter<TQuery>().Deserialize(ref reader, formatterResolver);
    }
  }
}
