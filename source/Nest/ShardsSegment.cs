// Decompiled with JetBrains decompiler
// Type: Nest.ShardsSegment
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (ShardsSegment.Json))]
  public class ShardsSegment
  {
    [DataMember(Name = "num_committed_segments")]
    public int CommittedSegments { get; internal set; }

    [DataMember(Name = "routing")]
    public ShardSegmentRouting Routing { get; internal set; }

    [DataMember(Name = "num_search_segments")]
    public int SearchSegments { get; internal set; }

    [DataMember(Name = "segments")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, Segment>))]
    public IReadOnlyDictionary<string, Segment> Segments { get; internal set; } = EmptyReadOnly<string, Segment>.Dictionary;

    internal class Json : IJsonFormatter<ShardsSegment>, IJsonFormatter
    {
      public ShardsSegment Deserialize(
        ref Elasticsearch.Net.Utf8Json.JsonReader reader,
        IJsonFormatterResolver formatterResolver)
      {
        IJsonFormatter<ShardsSegment> formatter = DynamicObjectResolver.AllowPrivateExcludeNullCamelCase.GetFormatter<ShardsSegment>();
        ShardsSegment shardsSegment = (ShardsSegment) null;
        if (reader.GetCurrentJsonToken() == JsonToken.BeginArray)
        {
          int count = 0;
          while (reader.ReadIsInArray(ref count))
          {
            if (count == 1)
              shardsSegment = formatter.Deserialize(ref reader, formatterResolver);
            else
              reader.ReadNextBlock();
          }
        }
        else
          shardsSegment = formatter.Deserialize(ref reader, formatterResolver);
        return shardsSegment;
      }

      public void Serialize(
        ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
        ShardsSegment value,
        IJsonFormatterResolver formatterResolver)
      {
        DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<ShardsSegment>().Serialize(ref writer, value, formatterResolver);
      }
    }
  }
}
