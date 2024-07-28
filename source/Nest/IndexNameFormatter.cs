// Decompiled with JetBrains decompiler
// Type: Nest.IndexNameFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class IndexNameFormatter : 
    IJsonFormatter<IndexName>,
    IJsonFormatter,
    IObjectPropertyNameFormatter<IndexName>
  {
    public IndexName Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return (IndexName) reader.ReadString();
      reader.ReadNextBlock();
      return (IndexName) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      IndexName value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (IndexName) null)
      {
        writer.WriteNull();
      }
      else
      {
        string str = formatterResolver.GetConnectionSettings().Inferrer.IndexName(value);
        writer.WriteString(str);
      }
    }

    public void SerializeToPropertyName(
      ref JsonWriter writer,
      IndexName value,
      IJsonFormatterResolver formatterResolver)
    {
      this.Serialize(ref writer, value, formatterResolver);
    }

    public IndexName DeserializeFromPropertyName(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return this.Deserialize(ref reader, formatterResolver);
    }
  }
}
