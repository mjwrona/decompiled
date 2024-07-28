// Decompiled with JetBrains decompiler
// Type: Nest.PercentileRanksAggregationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class PercentileRanksAggregationFormatter : 
    IJsonFormatter<IPercentileRanksAggregation>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "hdr",
        0
      },
      {
        "tdigest",
        1
      },
      {
        "field",
        2
      },
      {
        "script",
        3
      },
      {
        "missing",
        4
      },
      {
        "meta",
        5
      },
      {
        "values",
        6
      },
      {
        "keyed",
        7
      },
      {
        "format",
        8
      }
    };

    public IPercentileRanksAggregation Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (IPercentileRanksAggregation) null;
      }
      int count = 0;
      PercentileRanksAggregation ranksAggregation = new PercentileRanksAggregation();
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (PercentileRanksAggregationFormatter.AutomataDictionary.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              ranksAggregation.Method = (IPercentilesMethod) formatterResolver.GetFormatter<HDRHistogramMethod>().Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              ranksAggregation.Method = (IPercentilesMethod) formatterResolver.GetFormatter<TDigestMethod>().Deserialize(ref reader, formatterResolver);
              continue;
            case 2:
              ranksAggregation.Field = (Field) reader.ReadString();
              continue;
            case 3:
              ranksAggregation.Script = formatterResolver.GetFormatter<IScript>().Deserialize(ref reader, formatterResolver);
              continue;
            case 4:
              ranksAggregation.Missing = new double?(reader.ReadDouble());
              continue;
            case 5:
              ranksAggregation.Meta = formatterResolver.GetFormatter<IDictionary<string, object>>().Deserialize(ref reader, formatterResolver);
              continue;
            case 6:
              ranksAggregation.Values = formatterResolver.GetFormatter<IEnumerable<double>>().Deserialize(ref reader, formatterResolver);
              continue;
            case 7:
              ranksAggregation.Keyed = new bool?(reader.ReadBoolean());
              continue;
            case 8:
              ranksAggregation.Format = reader.ReadString();
              continue;
            default:
              continue;
          }
        }
      }
      return (IPercentileRanksAggregation) ranksAggregation;
    }

    public void Serialize(
      ref JsonWriter writer,
      IPercentileRanksAggregation value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        bool flag = false;
        if (value.Meta != null && value.Meta.Any<KeyValuePair<string, object>>())
        {
          writer.WritePropertyName("meta");
          formatterResolver.GetFormatter<IDictionary<string, object>>().Serialize(ref writer, value.Meta, formatterResolver);
          flag = true;
        }
        if (value.Field != (Field) null)
        {
          if (flag)
            writer.WriteValueSeparator();
          IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
          writer.WritePropertyName("field");
          writer.WriteString(connectionSettings.Inferrer.Field(value.Field));
          flag = true;
        }
        if (value.Script != null)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("script");
          formatterResolver.GetFormatter<IScript>().Serialize(ref writer, value.Script, formatterResolver);
          flag = true;
        }
        double? nullable;
        if (value.Method != null)
        {
          if (flag)
            writer.WriteValueSeparator();
          switch (value.Method)
          {
            case ITDigestMethod tdigestMethod:
              writer.WritePropertyName("tdigest");
              writer.WriteBeginObject();
              nullable = tdigestMethod.Compression;
              if (nullable.HasValue)
              {
                writer.WritePropertyName("compression");
                ref JsonWriter local = ref writer;
                nullable = tdigestMethod.Compression;
                double num = nullable.Value;
                local.WriteDouble(num);
              }
              writer.WriteEndObject();
              break;
            case IHDRHistogramMethod hdrHistogramMethod:
              writer.WritePropertyName("hdr");
              writer.WriteBeginObject();
              int? significantValueDigits = hdrHistogramMethod.NumberOfSignificantValueDigits;
              if (significantValueDigits.HasValue)
              {
                writer.WritePropertyName("number_of_significant_value_digits");
                ref JsonWriter local = ref writer;
                significantValueDigits = hdrHistogramMethod.NumberOfSignificantValueDigits;
                int num = significantValueDigits.Value;
                local.WriteInt32(num);
              }
              writer.WriteEndObject();
              break;
          }
          flag = true;
        }
        nullable = value.Missing;
        if (nullable.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("missing");
          ref JsonWriter local = ref writer;
          nullable = value.Missing;
          double num = nullable.Value;
          local.WriteDouble(num);
          flag = true;
        }
        if (value.Values != null && value.Values.Any<double>())
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("values");
          formatterResolver.GetFormatter<IEnumerable<double>>().Serialize(ref writer, value.Values, formatterResolver);
          flag = true;
        }
        bool? keyed = value.Keyed;
        if (keyed.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("keyed");
          ref JsonWriter local = ref writer;
          keyed = value.Keyed;
          int num = keyed.Value ? 1 : 0;
          local.WriteBoolean(num != 0);
          flag = true;
        }
        if (!string.IsNullOrEmpty(value.Format))
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("format");
          writer.WriteString(value.Format);
        }
        writer.WriteEndObject();
      }
    }
  }
}
