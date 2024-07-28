// Decompiled with JetBrains decompiler
// Type: Nest.AutoFollowPatternFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System.Collections.Generic;

namespace Nest
{
  internal class AutoFollowPatternFormatter : 
    IJsonFormatter<IReadOnlyDictionary<string, AutoFollowPattern>>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary AutoFollowPatternFields = new AutomataDictionary()
    {
      {
        "name",
        0
      },
      {
        "pattern",
        1
      }
    };

    public IReadOnlyDictionary<string, AutoFollowPattern> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IReadOnlyDictionary<string, AutoFollowPattern>) null;
      int count1 = 0;
      Dictionary<string, AutoFollowPattern> dictionary = new Dictionary<string, AutoFollowPattern>();
      while (reader.ReadIsInArray(ref count1))
      {
        int count2 = 0;
        string key = (string) null;
        AutoFollowPattern autoFollowPattern = (AutoFollowPattern) null;
        IJsonFormatter<AutoFollowPattern> formatter = formatterResolver.GetFormatter<AutoFollowPattern>();
        while (reader.ReadIsInObject(ref count2))
        {
          int num;
          if (AutoFollowPatternFormatter.AutoFollowPatternFields.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
          {
            switch (num)
            {
              case 0:
                key = reader.ReadString();
                break;
              case 1:
                autoFollowPattern = formatter.Deserialize(ref reader, formatterResolver);
                break;
            }
          }
          else
            reader.ReadNextBlock();
          if (key != null && autoFollowPattern != null)
            dictionary.Add(key, autoFollowPattern);
        }
      }
      return (IReadOnlyDictionary<string, AutoFollowPattern>) dictionary;
    }

    public void Serialize(
      ref JsonWriter writer,
      IReadOnlyDictionary<string, AutoFollowPattern> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        int num = 0;
        IJsonFormatter<IAutoFollowPattern> formatter = formatterResolver.GetFormatter<IAutoFollowPattern>();
        writer.WriteBeginArray();
        foreach (KeyValuePair<string, AutoFollowPattern> keyValuePair in (IEnumerable<KeyValuePair<string, AutoFollowPattern>>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WriteBeginObject();
          writer.WritePropertyName("name");
          writer.WriteString(keyValuePair.Key);
          writer.WriteValueSeparator();
          writer.WritePropertyName("pattern");
          formatter.Serialize(ref writer, (IAutoFollowPattern) keyValuePair.Value, formatterResolver);
          writer.WriteEndObject();
          ++num;
        }
        writer.WriteEndArray();
      }
    }
  }
}
