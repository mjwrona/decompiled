// Decompiled with JetBrains decompiler
// Type: Nest.RelationNameFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class RelationNameFormatter : 
    IJsonFormatter<RelationName>,
    IJsonFormatter,
    IObjectPropertyNameFormatter<RelationName>
  {
    public RelationName Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return (RelationName) reader.ReadString();
      reader.ReadNextBlock();
      return (RelationName) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      RelationName value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (RelationName) null)
      {
        writer.WriteNull();
      }
      else
      {
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        writer.WriteString(connectionSettings.Inferrer.RelationName(value));
      }
    }

    public void SerializeToPropertyName(
      ref JsonWriter writer,
      RelationName value,
      IJsonFormatterResolver formatterResolver)
    {
      this.Serialize(ref writer, value, formatterResolver);
    }

    public RelationName DeserializeFromPropertyName(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return this.Deserialize(ref reader, formatterResolver);
    }
  }
}
