// Decompiled with JetBrains decompiler
// Type: Nest.MinimumShouldMatchFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class MinimumShouldMatchFormatter : IJsonFormatter<MinimumShouldMatch>, IJsonFormatter
  {
    public MinimumShouldMatch Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.Number:
          return new MinimumShouldMatch(reader.ReadInt32());
        case JsonToken.String:
          return new MinimumShouldMatch(reader.ReadString());
        default:
          throw new Exception(string.Format("Expected {0} or {1} but got {2}", (object) "String", (object) "Number", (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      MinimumShouldMatch value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Tag)
        {
          case 0:
            writer.WriteInt32(value.Item1.Value);
            break;
          case 1:
            writer.WriteString(value.Item2);
            break;
        }
      }
    }
  }
}
