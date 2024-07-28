// Decompiled with JetBrains decompiler
// Type: Nest.StringDoubleFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class StringDoubleFormatter : IJsonFormatter<double>, IJsonFormatter
  {
    public double Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.Number:
          return reader.ReadDouble();
        case JsonToken.String:
          string s = reader.ReadString();
          if (s.Equals("Infinity", StringComparison.Ordinal))
            return double.PositiveInfinity;
          if (s.Equals("-Infinity", StringComparison.Ordinal))
            return double.NegativeInfinity;
          double result;
          if (!double.TryParse(s, out result))
            throw new JsonParsingException("Cannot parse " + typeof (double).FullName + " from: " + s);
          return result;
        case JsonToken.Null:
          throw new JsonParsingException(string.Format("Cannot parse non-nullable double value from: {0}.", (object) currentJsonToken));
        default:
          throw new JsonParsingException(string.Format("Cannot parse {0} from: {1}", (object) typeof (double).FullName, (object) currentJsonToken));
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      double value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WriteDouble(value);
    }
  }
}
