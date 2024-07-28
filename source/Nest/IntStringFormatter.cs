// Decompiled with JetBrains decompiler
// Type: Nest.IntStringFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Globalization;

namespace Nest
{
  internal class IntStringFormatter : IJsonFormatter<string>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      string value,
      IJsonFormatterResolver formatterResolver)
    {
      int result;
      if (!int.TryParse(value, out result))
        throw new InvalidOperationException("expected a int string value, but found " + value);
      writer.WriteInt32(result);
    }

    public string Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.Number:
          return reader.ReadInt32().ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case JsonToken.String:
          return reader.ReadString();
        default:
          throw new JsonParsingException(string.Format("expected string or int but found {0}", (object) reader.GetCurrentJsonToken()));
      }
    }
  }
}
