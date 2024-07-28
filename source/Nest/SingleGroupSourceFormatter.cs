// Decompiled with JetBrains decompiler
// Type: Nest.SingleGroupSourceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class SingleGroupSourceFormatter : IJsonFormatter<ISingleGroupSource>, IJsonFormatter
  {
    private static readonly AutomataDictionary GroupSource = new AutomataDictionary()
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
      ISingleGroupSource value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        if (!(value is ITermsGroupSource termsGroupSource))
        {
          if (!(value is IDateHistogramGroupSource histogramGroupSource2))
          {
            if (!(value is IHistogramGroupSource histogramGroupSource1))
            {
              if (!(value is IGeoTileGridGroupSource tileGridGroupSource))
                throw new JsonParsingException("Unknown ISingleGroupSource: " + value.GetType().Name);
              writer.WritePropertyName("geotile_grid");
              SingleGroupSourceFormatter.Serialize<IGeoTileGridGroupSource>(ref writer, tileGridGroupSource, formatterResolver);
            }
            else
            {
              writer.WritePropertyName("histogram");
              SingleGroupSourceFormatter.Serialize<IHistogramGroupSource>(ref writer, histogramGroupSource1, formatterResolver);
            }
          }
          else
          {
            writer.WritePropertyName("date_histogram");
            SingleGroupSourceFormatter.Serialize<IDateHistogramGroupSource>(ref writer, histogramGroupSource2, formatterResolver);
          }
        }
        else
        {
          writer.WritePropertyName("terms");
          SingleGroupSourceFormatter.Serialize<ITermsGroupSource>(ref writer, termsGroupSource, formatterResolver);
        }
        writer.WriteEndObject();
      }
    }

    private static void Serialize<TGroupSource>(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      TGroupSource value,
      IJsonFormatterResolver formatterResolver)
      where TGroupSource : ISingleGroupSource
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<TGroupSource>().Serialize(ref writer, value, formatterResolver);
    }

    public ISingleGroupSource Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (ISingleGroupSource) null;
      }
      reader.ReadIsBeginObjectWithVerify();
      ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
      ISingleGroupSource singleGroupSource = (ISingleGroupSource) null;
      int num;
      if (!SingleGroupSourceFormatter.GroupSource.TryGetValue(segment, out num))
        throw new JsonParsingException("Unknown ISingleGroupSource: " + segment.Utf8String());
      switch (num)
      {
        case 0:
          singleGroupSource = (ISingleGroupSource) formatterResolver.GetFormatter<TermsGroupSource>().Deserialize(ref reader, formatterResolver);
          break;
        case 1:
          singleGroupSource = (ISingleGroupSource) formatterResolver.GetFormatter<DateHistogramGroupSource>().Deserialize(ref reader, formatterResolver);
          break;
        case 2:
          singleGroupSource = (ISingleGroupSource) formatterResolver.GetFormatter<HistogramGroupSource>().Deserialize(ref reader, formatterResolver);
          break;
        case 3:
          singleGroupSource = (ISingleGroupSource) formatterResolver.GetFormatter<GeoTileGridGroupSource>().Deserialize(ref reader, formatterResolver);
          break;
      }
      reader.ReadIsEndObjectWithVerify();
      return singleGroupSource;
    }
  }
}
