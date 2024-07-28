// Decompiled with JetBrains decompiler
// Type: Nest.IndicesMultiSyntaxFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class IndicesMultiSyntaxFormatter : IJsonFormatter<Indices>, IJsonFormatter
  {
    public Indices Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return (Indices) reader.ReadString();
      reader.ReadNextBlock();
      return (Indices) null;
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
            writer.WriteString("_all");
            break;
          case 1:
            IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
            writer.WriteString(((IUrlParameter) value).GetString((IConnectionConfigurationValues) connectionSettings));
            break;
        }
      }
    }
  }
}
