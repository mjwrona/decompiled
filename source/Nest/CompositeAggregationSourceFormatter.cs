// Decompiled with JetBrains decompiler
// Type: Nest.CompositeAggregationSourceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class CompositeAggregationSourceFormatter : 
    IJsonFormatter<ICompositeAggregationSource>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary AggregationSource = new AutomataDictionary()
    {
      {
        "terms",
        0
      },
      {
        "date_histogram",
        1
      },
      {
        "histogram",
        2
      },
      {
        "geotile_grid",
        3
      }
    };

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ICompositeAggregationSource value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteBeginObject();
      writer.WritePropertyName(value.Name);
      writer.WriteBeginObject();
      writer.WritePropertyName(value.SourceType);
      switch (value)
      {
        case ITermsCompositeAggregationSource aggregationSource1:
          CompositeAggregationSourceFormatter.Serialize<ITermsCompositeAggregationSource>(ref writer, aggregationSource1, formatterResolver);
          break;
        case IDateHistogramCompositeAggregationSource aggregationSource2:
          CompositeAggregationSourceFormatter.Serialize<IDateHistogramCompositeAggregationSource>(ref writer, aggregationSource2, formatterResolver);
          break;
        case IHistogramCompositeAggregationSource aggregationSource3:
          CompositeAggregationSourceFormatter.Serialize<IHistogramCompositeAggregationSource>(ref writer, aggregationSource3, formatterResolver);
          break;
        case IGeoTileGridCompositeAggregationSource aggregationSource4:
          CompositeAggregationSourceFormatter.Serialize<IGeoTileGridCompositeAggregationSource>(ref writer, aggregationSource4, formatterResolver);
          break;
        default:
          this.Serialize(ref writer, value, formatterResolver);
          break;
      }
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void Serialize<TCompositeAggregationSource>(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      TCompositeAggregationSource value,
      IJsonFormatterResolver formatterResolver)
      where TCompositeAggregationSource : ICompositeAggregationSource
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<TCompositeAggregationSource>().Serialize(ref writer, value, formatterResolver);
    }

    public ICompositeAggregationSource Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (ICompositeAggregationSource) null;
      reader.ReadIsBeginObjectWithVerify();
      string str = reader.ReadPropertyName();
      reader.ReadIsBeginObjectWithVerify();
      ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
      ICompositeAggregationSource aggregationSource = (ICompositeAggregationSource) null;
      int num;
      if (!CompositeAggregationSourceFormatter.AggregationSource.TryGetValue(segment, out num))
        throw new Exception("Unknown ICompositeAggregationSource: " + segment.Utf8String());
      switch (num)
      {
        case 0:
          aggregationSource = (ICompositeAggregationSource) formatterResolver.GetFormatter<TermsCompositeAggregationSource>().Deserialize(ref reader, formatterResolver);
          break;
        case 1:
          aggregationSource = (ICompositeAggregationSource) formatterResolver.GetFormatter<DateHistogramCompositeAggregationSource>().Deserialize(ref reader, formatterResolver);
          break;
        case 2:
          aggregationSource = (ICompositeAggregationSource) formatterResolver.GetFormatter<HistogramCompositeAggregationSource>().Deserialize(ref reader, formatterResolver);
          break;
        case 3:
          aggregationSource = (ICompositeAggregationSource) formatterResolver.GetFormatter<GeoTileGridCompositeAggregationSource>().Deserialize(ref reader, formatterResolver);
          break;
      }
      reader.ReadIsEndObjectWithVerify();
      reader.ReadIsEndObjectWithVerify();
      aggregationSource.Name = str;
      return aggregationSource;
    }
  }
}
