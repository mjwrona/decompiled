// Decompiled with JetBrains decompiler
// Type: Nest.AggregateDictionaryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class AggregateDictionaryFormatter : IJsonFormatter<AggregateDictionary>, IJsonFormatter
  {
    private static readonly AggregateFormatter Formatter = new AggregateFormatter();

    public AggregateDictionary Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      Dictionary<string, IAggregate> dictionary = new Dictionary<string, IAggregate>();
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return new AggregateDictionary((IReadOnlyDictionary<string, IAggregate>) dictionary);
      }
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        string key = reader.ReadPropertyName();
        if (key.IsNullOrEmpty())
        {
          reader.ReadNextBlock();
        }
        else
        {
          string[] tokens = AggregateDictionary.TypedKeyTokens(key);
          if (tokens.Length == 1)
            AggregateDictionaryFormatter.ParseAggregate(ref reader, formatterResolver, tokens[0], dictionary);
          else
            AggregateDictionaryFormatter.ReadAggregate(ref reader, formatterResolver, tokens, dictionary);
        }
      }
      return new AggregateDictionary((IReadOnlyDictionary<string, IAggregate>) dictionary);
    }

    public void Serialize(
      ref JsonWriter writer,
      AggregateDictionary value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    private static void ReadAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      string[] tokens,
      Dictionary<string, IAggregate> dictionary)
    {
      string token = tokens[1];
      switch (tokens[0])
      {
        case "geo_centroid":
          AggregateDictionaryFormatter.ReadAggregate<GeoCentroidAggregate>(ref reader, formatterResolver, token, dictionary);
          break;
        case "geo_line":
          AggregateDictionaryFormatter.ReadAggregate<GeoLineAggregate>(ref reader, formatterResolver, token, dictionary);
          break;
        default:
          AggregateDictionaryFormatter.ParseAggregate(ref reader, formatterResolver, token, dictionary);
          break;
      }
    }

    private static void ReadAggregate<TAggregate>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      string name,
      Dictionary<string, IAggregate> dictionary)
      where TAggregate : IAggregate
    {
      TAggregate aggregate = formatterResolver.GetFormatter<TAggregate>().Deserialize(ref reader, formatterResolver);
      dictionary.Add(name, (IAggregate) aggregate);
    }

    private static void ParseAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      string name,
      Dictionary<string, IAggregate> dictionary)
    {
      IAggregate aggregate = AggregateDictionaryFormatter.Formatter.Deserialize(ref reader, formatterResolver);
      dictionary.Add(name, aggregate);
    }
  }
}
