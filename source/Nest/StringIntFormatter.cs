// Decompiled with JetBrains decompiler
// Type: Nest.StringIntFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class StringIntFormatter : IJsonFormatter<int>, IJsonFormatter
  {
    public int Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.Number:
          return reader.ReadInt32();
        case JsonToken.String:
          string s = reader.ReadString();
          int result;
          if (!int.TryParse(s, out result))
            throw new JsonParsingException("Cannot parse " + typeof (int).FullName + " from: " + s);
          return result;
        default:
          throw new JsonParsingException(string.Format("Cannot parse {0} from: {1}", (object) typeof (int).FullName, (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      int value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteInt32(value);
    }
  }
}
