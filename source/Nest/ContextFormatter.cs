// Decompiled with JetBrains decompiler
// Type: Nest.ContextFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class ContextFormatter : IJsonFormatter<Context>, IJsonFormatter
  {
    public Context Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      Union<string, GeoLocation> union = formatterResolver.GetFormatter<Union<string, GeoLocation>>().Deserialize(ref reader, formatterResolver);
      switch (union.Tag)
      {
        case 0:
          return new Context(union.Item1);
        case 1:
          return new Context(union.Item2);
        default:
          return (Context) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Context value,
      IJsonFormatterResolver formatterResolver)
    {
      formatterResolver.GetFormatter<Union<string, GeoLocation>>().Serialize(ref writer, (Union<string, GeoLocation>) value, formatterResolver);
    }
  }
}
