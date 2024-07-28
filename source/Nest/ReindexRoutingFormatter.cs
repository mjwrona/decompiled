// Decompiled with JetBrains decompiler
// Type: Nest.ReindexRoutingFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class ReindexRoutingFormatter : IJsonFormatter<ReindexRouting>, IJsonFormatter
  {
    public ReindexRouting Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      string newRoutingValue = reader.ReadString();
      switch (newRoutingValue)
      {
        case "keep":
          return ReindexRouting.Keep;
        case "discard":
          return ReindexRouting.Discard;
        default:
          return new ReindexRouting(newRoutingValue);
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      ReindexRouting value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        writer.WriteString(value.ToString());
    }
  }
}
