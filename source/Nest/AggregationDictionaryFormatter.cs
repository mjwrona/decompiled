// Decompiled with JetBrains decompiler
// Type: Nest.AggregationDictionaryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class AggregationDictionaryFormatter : 
    IJsonFormatter<AggregationDictionary>,
    IJsonFormatter
  {
    private static readonly VerbatimDictionaryInterfaceKeysFormatter<string, IAggregationContainer> DictionaryKeysFormatter = new VerbatimDictionaryInterfaceKeysFormatter<string, IAggregationContainer>();

    public AggregationDictionary Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new AggregationDictionary(AggregationDictionaryFormatter.DictionaryKeysFormatter.Deserialize(ref reader, formatterResolver));
    }

    public void Serialize(
      ref JsonWriter writer,
      AggregationDictionary value,
      IJsonFormatterResolver formatterResolver)
    {
      AggregationDictionaryFormatter.DictionaryKeysFormatter.Serialize(ref writer, (IDictionary<string, IAggregationContainer>) value, formatterResolver);
    }
  }
}
