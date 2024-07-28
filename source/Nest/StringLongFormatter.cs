// Decompiled with JetBrains decompiler
// Type: Nest.StringLongFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class StringLongFormatter : IJsonFormatter<long>, IJsonFormatter
  {
    public long Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.Number:
          return reader.ReadInt64();
        case JsonToken.String:
          string s = reader.ReadString();
          long result;
          if (!long.TryParse(s, out result))
            throw new JsonParsingException("Cannot parse " + typeof (long).FullName + " from: " + s);
          return result;
        default:
          throw new JsonParsingException(string.Format("Cannot parse {0} from: {1}", (object) typeof (long).FullName, (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      long value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteInt64(value);
    }
  }
}
