// Decompiled with JetBrains decompiler
// Type: Nest.BucketsPathFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class BucketsPathFormatter : IJsonFormatter<IBucketsPath>, IJsonFormatter
  {
    public IBucketsPath Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          return (IBucketsPath) new MultiBucketsPath(formatterResolver.GetFormatter<Dictionary<string, string>>().Deserialize(ref reader, formatterResolver));
        case JsonToken.String:
          return (IBucketsPath) new SingleBucketsPath(reader.ReadString());
        default:
          return (IBucketsPath) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IBucketsPath value,
      IJsonFormatterResolver formatterResolver)
    {
      switch (value)
      {
        case SingleBucketsPath singleBucketsPath:
          writer.WriteString(singleBucketsPath.BucketsPath);
          break;
        case MultiBucketsPath multiBucketsPath:
          writer.WriteBeginObject();
          int num = 0;
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) multiBucketsPath)
          {
            if (num != 0)
              writer.WriteValueSeparator();
            writer.WritePropertyName(keyValuePair.Key);
            writer.WriteString(keyValuePair.Value);
            ++num;
          }
          writer.WriteEndObject();
          break;
        default:
          writer.WriteNull();
          break;
      }
    }
  }
}
