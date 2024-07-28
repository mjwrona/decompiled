// Decompiled with JetBrains decompiler
// Type: Nest.StopWordsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class StopWordsFormatter : IJsonFormatter<StopWords>, IJsonFormatter
  {
    public StopWords Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => reader.GetCurrentJsonToken() == JsonToken.BeginArray ? new StopWords(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver)) : new StopWords(reader.ReadString());

    public void Serialize(
      ref JsonWriter writer,
      StopWords value,
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
            writer.WriteString(value.Item1);
            break;
          case 1:
            formatterResolver.GetFormatter<IEnumerable<string>>().Serialize(ref writer, value.Item2, formatterResolver);
            break;
        }
      }
    }
  }
}
