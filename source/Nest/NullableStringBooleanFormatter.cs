// Decompiled with JetBrains decompiler
// Type: Nest.NullableStringBooleanFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class NullableStringBooleanFormatter : IJsonFormatter<bool?>, IJsonFormatter
  {
    public bool? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.String:
          string str = reader.ReadString();
          bool result;
          if (!bool.TryParse(str, out result))
            throw new JsonParsingException("Cannot parse " + typeof (bool).FullName + " from: " + str);
          return new bool?(result);
        case JsonToken.True:
        case JsonToken.False:
          return new bool?(reader.ReadBoolean());
        case JsonToken.Null:
          reader.ReadNext();
          return new bool?();
        default:
          throw new JsonParsingException(string.Format("Cannot parse {0} from: {1}", (object) typeof (bool).FullName, (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      bool? value,
      IJsonFormatterResolver formatterResolver)
    {
      if (!value.HasValue)
        writer.WriteNull();
      else
        writer.WriteBoolean(value.Value);
    }
  }
}
