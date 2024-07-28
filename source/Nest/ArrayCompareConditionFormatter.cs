// Decompiled with JetBrains decompiler
// Type: Nest.ArrayCompareConditionFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class ArrayCompareConditionFormatter : 
    IJsonFormatter<IArrayCompareCondition>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary ComparisonProperties = new AutomataDictionary()
    {
      {
        "quantifier",
        0
      },
      {
        "value",
        1
      }
    };
    private static readonly AutomataDictionary Comparisons = new AutomataDictionary()
    {
      {
        "eq",
        0
      },
      {
        "not_eq",
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
        "lt",
        4
      },
      {
        "lte",
        5
      }
    };

    public IArrayCompareCondition Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (IArrayCompareCondition) null;
      }
      int count1 = 0;
      string arrayPath = (string) null;
      string path = (string) null;
      int num1 = -1;
      Quantifier? nullable = new Quantifier?();
      object obj = (object) null;
      while (reader.ReadIsInObject(ref count1))
      {
        arrayPath = reader.ReadPropertyName();
        int count2 = 0;
        while (reader.ReadIsInObject(ref count2))
        {
          ArraySegment<byte> bytes1 = reader.ReadPropertyNameSegmentRaw();
          if (ArrayCompareConditionFormatter.Comparisons.TryGetValue(bytes1, out num1))
          {
            int count3 = 0;
            while (reader.ReadIsInObject(ref count3))
            {
              ArraySegment<byte> bytes2 = reader.ReadPropertyNameSegmentRaw();
              int num2;
              if (ArrayCompareConditionFormatter.ComparisonProperties.TryGetValue(bytes2, out num2))
              {
                switch (num2)
                {
                  case 0:
                    nullable = new Quantifier?(formatterResolver.GetFormatter<Quantifier>().Deserialize(ref reader, formatterResolver));
                    continue;
                  case 1:
                    obj = formatterResolver.GetFormatter<object>().Deserialize(ref reader, formatterResolver);
                    continue;
                  default:
                    continue;
                }
              }
              else
                reader.ReadNextBlock();
            }
          }
          else
            path = reader.ReadString();
        }
      }
      switch (num1)
      {
        case 0:
          EqualArrayCondition equalArrayCondition1 = new EqualArrayCondition(arrayPath, path, obj);
          equalArrayCondition1.Quantifier = nullable;
          return (IArrayCompareCondition) equalArrayCondition1;
        case 1:
          NotEqualArrayCondition equalArrayCondition2 = new NotEqualArrayCondition(arrayPath, path, obj);
          equalArrayCondition2.Quantifier = nullable;
          return (IArrayCompareCondition) equalArrayCondition2;
        case 2:
          GreaterThanArrayCondition thanArrayCondition1 = new GreaterThanArrayCondition(arrayPath, path, obj);
          thanArrayCondition1.Quantifier = nullable;
          return (IArrayCompareCondition) thanArrayCondition1;
        case 3:
          GreaterThanOrEqualArrayCondition equalArrayCondition3 = new GreaterThanOrEqualArrayCondition(arrayPath, path, obj);
          equalArrayCondition3.Quantifier = nullable;
          return (IArrayCompareCondition) equalArrayCondition3;
        case 4:
          LowerThanArrayCondition thanArrayCondition2 = new LowerThanArrayCondition(arrayPath, path, obj);
          thanArrayCondition2.Quantifier = nullable;
          return (IArrayCompareCondition) thanArrayCondition2;
        case 5:
          LowerThanOrEqualArrayCondition equalArrayCondition4 = new LowerThanOrEqualArrayCondition(arrayPath, path, obj);
          equalArrayCondition4.Quantifier = nullable;
          return (IArrayCompareCondition) equalArrayCondition4;
        default:
          return (IArrayCompareCondition) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IArrayCompareCondition value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.ArrayPath.IsNullOrEmpty())
        return;
      writer.WriteBeginObject();
      writer.WritePropertyName(value.ArrayPath);
      writer.WriteBeginObject();
      writer.WritePropertyName("path");
      writer.WriteString(value.Path);
      writer.WriteValueSeparator();
      writer.WritePropertyName(value.Comparison);
      writer.WriteBeginObject();
      if (value.Quantifier.HasValue)
      {
        writer.WritePropertyName("quantifier");
        formatterResolver.GetFormatter<Quantifier>().Serialize(ref writer, value.Quantifier.Value, formatterResolver);
      }
      writer.WritePropertyName(nameof (value));
      formatterResolver.GetFormatter<object>().Serialize(ref writer, value.Value, formatterResolver);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }
  }
}
