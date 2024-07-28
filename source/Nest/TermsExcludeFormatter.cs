// Decompiled with JetBrains decompiler
// Type: Nest.TermsExcludeFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class TermsExcludeFormatter : IJsonFormatter<TermsExclude>, IJsonFormatter
  {
    public TermsExclude Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginArray:
          return new TermsExclude(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
        case JsonToken.String:
          return new TermsExclude(reader.ReadString());
        case JsonToken.Null:
          reader.ReadNext();
          return (TermsExclude) null;
        default:
          throw new Exception(string.Format("Unexpected token {0} when deserializing {1}", (object) currentJsonToken, (object) "TermsInclude"));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      TermsExclude value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else if (value.Values != null)
        formatterResolver.GetFormatter<IEnumerable<string>>().Serialize(ref writer, value.Values, formatterResolver);
      else
        writer.WriteString(value.Pattern);
    }
  }
}
