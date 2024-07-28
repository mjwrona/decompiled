// Decompiled with JetBrains decompiler
// Type: Nest.IdStringFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class IdStringFormatter : IJsonFormatter<string>, IJsonFormatter
  {
    public string Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => reader.GetCurrentJsonToken() != JsonToken.Number ? reader.ReadString() : reader.ReadInt64().ToString();

    public void Serialize(
      ref JsonWriter writer,
      string value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteString(value);
    }
  }
}
