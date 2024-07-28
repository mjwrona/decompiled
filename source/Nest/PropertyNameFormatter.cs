// Decompiled with JetBrains decompiler
// Type: Nest.PropertyNameFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class PropertyNameFormatter : 
    IJsonFormatter<PropertyName>,
    IJsonFormatter,
    IObjectPropertyNameFormatter<PropertyName>
  {
    public PropertyName Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return (PropertyName) reader.ReadString();
      reader.ReadNextBlock();
      return (PropertyName) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      PropertyName value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (PropertyName) null)
      {
        writer.WriteNull();
      }
      else
      {
        Inferrer inferrer = formatterResolver.GetConnectionSettings().Inferrer;
        writer.WriteString(inferrer.PropertyName(value));
      }
    }

    public void SerializeToPropertyName(
      ref JsonWriter writer,
      PropertyName value,
      IJsonFormatterResolver formatterResolver)
    {
      this.Serialize(ref writer, value, formatterResolver);
    }

    public PropertyName DeserializeFromPropertyName(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return this.Deserialize(ref reader, formatterResolver);
    }
  }
}
