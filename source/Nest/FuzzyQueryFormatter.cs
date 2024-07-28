// Decompiled with JetBrains decompiler
// Type: Nest.FuzzyQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class FuzzyQueryFormatter : FieldNameQueryFormatter<FuzzyQuery, IFuzzyQuery>
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "value",
        0
      },
      {
        "fuzziness",
        1
      },
      {
        "prefix_length",
        2
      },
      {
        "max_expansions",
        3
      },
      {
        "transpositions",
        4
      },
      {
        "rewrite",
        5
      },
      {
        "_name",
        6
      },
      {
        "boost",
        7
      }
    };

    public override void SerializeInternal(
      ref JsonWriter writer,
      IFuzzyQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      switch (value)
      {
        case IFuzzyStringQuery fuzzyStringQuery:
          formatterResolver.GetFormatter<IFuzzyStringQuery>().Serialize(ref writer, fuzzyStringQuery, formatterResolver);
          break;
        case IFuzzyDateQuery fuzzyDateQuery:
          formatterResolver.GetFormatter<IFuzzyDateQuery>().Serialize(ref writer, fuzzyDateQuery, formatterResolver);
          break;
        case IFuzzyNumericQuery fuzzyNumericQuery:
          formatterResolver.GetFormatter<IFuzzyNumericQuery>().Serialize(ref writer, fuzzyNumericQuery, formatterResolver);
          break;
        default:
          base.SerializeInternal(ref writer, value, formatterResolver);
          break;
      }
    }

    public override IFuzzyQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (IFuzzyQuery) null;
      }
      int count1 = 0;
      IFuzzyQuery query = (IFuzzyQuery) null;
      string str1 = (string) null;
      double? nullable1 = new double?();
      MultiTermQueryRewrite termQueryRewrite = (MultiTermQueryRewrite) null;
      int? nullable2 = new int?();
      int? nullable3 = new int?();
      bool? nullable4 = new bool?();
      while (reader.ReadIsInObject(ref count1))
      {
        string str2 = reader.ReadPropertyName();
        ArraySegment<byte> arraySegment1 = new ArraySegment<byte>();
        int count2 = 0;
        while (reader.ReadIsInObject(ref count2))
        {
          ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
          int num;
          if (FuzzyQueryFormatter.Fields.TryGetValue(bytes, out num))
          {
            switch (num)
            {
              case 0:
                switch (reader.GetCurrentJsonToken())
                {
                  case JsonToken.Number:
                    FuzzyNumericQuery fuzzyNumericQuery = new FuzzyNumericQuery();
                    fuzzyNumericQuery.Field = (Field) str2;
                    fuzzyNumericQuery.Value = new double?(reader.ReadDouble());
                    query = (IFuzzyQuery) fuzzyNumericQuery;
                    break;
                  case JsonToken.String:
                    ArraySegment<byte> arraySegment2 = reader.ReadStringSegmentUnsafe();
                    DateTime dateTime;
                    if (arraySegment2.IsDateTime(formatterResolver, out dateTime))
                    {
                      FuzzyDateQuery fuzzyDateQuery = new FuzzyDateQuery();
                      fuzzyDateQuery.Field = (Field) str2;
                      fuzzyDateQuery.Value = new DateTime?(dateTime);
                      query = (IFuzzyQuery) fuzzyDateQuery;
                      break;
                    }
                    FuzzyQuery fuzzyQuery = new FuzzyQuery();
                    fuzzyQuery.Field = (Field) str2;
                    fuzzyQuery.Value = arraySegment2.Utf8String();
                    query = (IFuzzyQuery) fuzzyQuery;
                    break;
                }
                if (arraySegment1 != new ArraySegment<byte>())
                {
                  JsonReader reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
                  FuzzyQueryFormatter.SetFuzziness(ref reader1, query, formatterResolver);
                  continue;
                }
                continue;
              case 1:
                if (query != null)
                {
                  FuzzyQueryFormatter.SetFuzziness(ref reader, query, formatterResolver);
                  continue;
                }
                arraySegment1 = reader.ReadNextBlockSegment();
                continue;
              case 2:
                nullable2 = new int?(reader.ReadInt32());
                continue;
              case 3:
                nullable3 = new int?(reader.ReadInt32());
                continue;
              case 4:
                nullable4 = new bool?(reader.ReadBoolean());
                continue;
              case 5:
                termQueryRewrite = formatterResolver.GetFormatter<MultiTermQueryRewrite>().Deserialize(ref reader, formatterResolver);
                continue;
              case 6:
                str1 = reader.ReadString();
                continue;
              case 7:
                nullable1 = new double?(reader.ReadDouble());
                continue;
              default:
                continue;
            }
          }
        }
      }
      query.PrefixLength = nullable2;
      query.MaxExpansions = nullable3;
      query.Transpositions = nullable4;
      query.Rewrite = termQueryRewrite;
      query.Name = str1;
      query.Boost = nullable1;
      return query;
    }

    private static void SetFuzziness(
      ref JsonReader reader,
      IFuzzyQuery query,
      IJsonFormatterResolver formatterResolver)
    {
      switch (query)
      {
        case FuzzyDateQuery fuzzyDateQuery:
          fuzzyDateQuery.Fuzziness = formatterResolver.GetFormatter<Time>().Deserialize(ref reader, formatterResolver);
          break;
        case FuzzyNumericQuery fuzzyNumericQuery:
          fuzzyNumericQuery.Fuzziness = new double?(reader.ReadDouble());
          break;
        case FuzzyQuery fuzzyQuery:
          fuzzyQuery.Fuzziness = formatterResolver.GetFormatter<Fuzziness>().Deserialize(ref reader, formatterResolver);
          break;
      }
    }
  }
}
