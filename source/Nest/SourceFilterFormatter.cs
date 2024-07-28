// Decompiled with JetBrains decompiler
// Type: Nest.SourceFilterFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class SourceFilterFormatter : IJsonFormatter<ISourceFilter>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "includes",
        0
      },
      {
        "excludes",
        1
      }
    };

    public ISourceFilter Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      if (currentJsonToken == JsonToken.Null)
      {
        reader.ReadNext();
        return (ISourceFilter) null;
      }
      SourceFilter sourceFilter = new SourceFilter();
      if (currentJsonToken != JsonToken.BeginArray)
      {
        if (currentJsonToken == JsonToken.String)
        {
          sourceFilter.Includes = (Nest.Fields) new string[1]
          {
            reader.ReadString()
          };
        }
        else
        {
          int count = 0;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (SourceFilterFormatter.Fields.TryGetValue(bytes, out num))
            {
              Nest.Fields fields = formatterResolver.GetFormatter<Nest.Fields>().Deserialize(ref reader, formatterResolver);
              switch (num)
              {
                case 0:
                  sourceFilter.Includes = fields;
                  continue;
                case 1:
                  sourceFilter.Excludes = fields;
                  continue;
                default:
                  continue;
              }
            }
            else
              reader.ReadNextBlock();
          }
        }
      }
      else
      {
        string[] strArray = formatterResolver.GetFormatter<string[]>().Deserialize(ref reader, formatterResolver);
        sourceFilter.Includes = (Nest.Fields) strArray;
      }
      return (ISourceFilter) sourceFilter;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ISourceFilter value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<ISourceFilter>().Serialize(ref writer, value, formatterResolver);
    }
  }
}
