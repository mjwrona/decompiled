// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermQueryRewriteFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class MultiTermQueryRewriteFormatter : 
    IJsonFormatter<MultiTermQueryRewrite>,
    IJsonFormatter
  {
    public MultiTermQueryRewrite Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.String:
          return MultiTermQueryRewrite.Create(reader.ReadString());
        case JsonToken.Null:
          reader.ReadNext();
          return (MultiTermQueryRewrite) null;
        default:
          throw new Exception(string.Format("Invalid token type {0} to deserialize {1} from", (object) currentJsonToken, (object) "MultiTermQueryRewrite"));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      MultiTermQueryRewrite value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (MultiTermQueryRewrite) null)
        writer.WriteNull();
      else
        writer.WriteString(value.ToString());
    }
  }
}
