// Decompiled with JetBrains decompiler
// Type: Nest.TimeFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class TimeFormatter : IJsonFormatter<Time>, IJsonFormatter
  {
    public Time Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.Number:
          long milliseconds = reader.ReadInt64();
          switch (milliseconds)
          {
            case -1:
              return Time.MinusOne;
            case 0:
              return Time.Zero;
            default:
              return new Time((double) milliseconds);
          }
        case JsonToken.String:
          return new Time(reader.ReadString());
        default:
          reader.ReadNextBlock();
          return (Time) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Time value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == Time.MinusOne)
        writer.WriteInt32(-1);
      else if (value == Time.Zero)
      {
        writer.WriteInt32(0);
      }
      else
      {
        double? nullable = value.Factor;
        if (nullable.HasValue && value.Interval.HasValue)
        {
          writer.WriteString(value.ToString());
        }
        else
        {
          nullable = value.Milliseconds;
          if (!nullable.HasValue)
            return;
          ref JsonWriter local = ref writer;
          nullable = value.Milliseconds;
          long num = (long) nullable.Value;
          local.WriteInt64(num);
        }
      }
    }
  }
}
