// Decompiled with JetBrains decompiler
// Type: Nest.CompareConditionFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class CompareConditionFormatter : IJsonFormatter<ICompareCondition>, IJsonFormatter
  {
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

    public ICompareCondition Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (ICompareCondition) null;
      int count1 = 0;
      ICompareCondition compareCondition = (ICompareCondition) null;
      while (reader.ReadIsInObject(ref count1))
      {
        string path = reader.ReadPropertyName();
        int count2 = 0;
        while (reader.ReadIsInObject(ref count2))
        {
          ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
          object obj = formatterResolver.GetFormatter<object>().Deserialize(ref reader, formatterResolver);
          int num;
          if (CompareConditionFormatter.Comparisons.TryGetValue(bytes, out num))
          {
            switch (num)
            {
              case 0:
                compareCondition = (ICompareCondition) new EqualCondition(path, obj);
                continue;
              case 1:
                compareCondition = (ICompareCondition) new NotEqualCondition(path, (object) num);
                continue;
              case 2:
                compareCondition = (ICompareCondition) new GreaterThanCondition(path, (object) num);
                continue;
              case 3:
                compareCondition = (ICompareCondition) new GreaterThanOrEqualCondition(path, (object) num);
                continue;
              case 4:
                compareCondition = (ICompareCondition) new LowerThanCondition(path, (object) num);
                continue;
              case 5:
                compareCondition = (ICompareCondition) new LowerThanOrEqualCondition(path, (object) num);
                continue;
              default:
                continue;
            }
          }
        }
      }
      return compareCondition;
    }

    public void Serialize(
      ref JsonWriter writer,
      ICompareCondition value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.Path.IsNullOrEmpty())
        return;
      writer.WriteBeginObject();
      writer.WritePropertyName(value.Path);
      writer.WriteBeginObject();
      writer.WritePropertyName(value.Comparison);
      formatterResolver.GetFormatter<object>().Serialize(ref writer, value.Value, formatterResolver);
      writer.WriteEndObject();
      writer.WriteEndObject();
    }
  }
}
