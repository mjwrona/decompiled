// Decompiled with JetBrains decompiler
// Type: Nest.IncludeExcludeFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class IncludeExcludeFormatter : IJsonFormatter<IncludeExclude>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IncludeExclude value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else if (value.Values != null)
        formatterResolver.GetFormatter<IEnumerable<string>>().Serialize(ref writer, value.Values, formatterResolver);
      else
        writer.WriteString(value.Pattern);
    }

    public IncludeExclude Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginArray:
          return new IncludeExclude(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
        case JsonToken.String:
          return new IncludeExclude(reader.ReadString());
        case JsonToken.Null:
          reader.ReadNext();
          return (IncludeExclude) null;
        default:
          throw new Exception(string.Format("Unexpected token {0} when deserializing {1}", (object) currentJsonToken, (object) "IncludeExclude"));
      }
    }
  }
}
