// Decompiled with JetBrains decompiler
// Type: Nest.IndicesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class IndicesFormatter : IJsonFormatter<Indices>, IJsonFormatter
  {
    public Indices Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginArray:
          List<IndexName> indices = new List<IndexName>();
          int count = 0;
          while (reader.ReadIsInArray(ref count))
          {
            string str = reader.ReadString();
            indices.Add((IndexName) str);
          }
          return new Indices((IEnumerable<IndexName>) indices);
        case JsonToken.String:
          return (Indices) reader.ReadString();
        default:
          reader.ReadNextBlock();
          return (Indices) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Indices value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (Indices) null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Tag)
        {
          case 0:
            writer.WriteBeginArray();
            writer.WriteString("_all");
            writer.WriteEndArray();
            break;
          case 1:
            IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
            writer.WriteBeginArray();
            for (int index1 = 0; index1 < value.Item2.Indices.Count; ++index1)
            {
              if (index1 > 0)
                writer.WriteValueSeparator();
              IndexName index2 = value.Item2.Indices[index1];
              writer.WriteString(index2.GetString((IConnectionConfigurationValues) connectionSettings));
            }
            writer.WriteEndArray();
            break;
        }
      }
    }
  }
}
