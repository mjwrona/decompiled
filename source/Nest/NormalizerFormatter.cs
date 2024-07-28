// Decompiled with JetBrains decompiler
// Type: Nest.NormalizerFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;

namespace Nest
{
  internal class NormalizerFormatter : IJsonFormatter<INormalizer>, IJsonFormatter
  {
    public INormalizer Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver) => (INormalizer) NormalizerFormatter.Deserialize<CustomNormalizer>(ref reader, formatterResolver);

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      INormalizer value,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<ICustomNormalizer>().Serialize(ref writer, value as ICustomNormalizer, formatterResolver);
    }

    private static TNormalizer Deserialize<TNormalizer>(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TNormalizer : INormalizer
    {
      return formatterResolver.GetFormatter<TNormalizer>().Deserialize(ref reader, formatterResolver);
    }
  }
}
