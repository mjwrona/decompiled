// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class GeoDistanceQueryFormatter : IJsonFormatter<IGeoDistanceQuery>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "_name",
        0
      },
      {
        "boost",
        1
      },
      {
        "validation_method",
        2
      },
      {
        "distance",
        3
      },
      {
        "distance_type",
        4
      },
      {
        "ignore_unmapped",
        5
      }
    };

    public IGeoDistanceQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IGeoDistanceQuery) null;
      GeoDistanceQuery geoDistanceQuery = new GeoDistanceQuery();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (GeoDistanceQueryFormatter.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              geoDistanceQuery.Name = reader.ReadString();
              continue;
            case 1:
              geoDistanceQuery.Boost = new double?(reader.ReadDouble());
              continue;
            case 2:
              geoDistanceQuery.ValidationMethod = new GeoValidationMethod?(formatterResolver.GetFormatter<GeoValidationMethod>().Deserialize(ref reader, formatterResolver));
              continue;
            case 3:
              geoDistanceQuery.Distance = formatterResolver.GetFormatter<Distance>().Deserialize(ref reader, formatterResolver);
              continue;
            case 4:
              geoDistanceQuery.DistanceType = new GeoDistanceType?(formatterResolver.GetFormatter<GeoDistanceType>().Deserialize(ref reader, formatterResolver));
              continue;
            case 5:
              geoDistanceQuery.IgnoreUnmapped = reader.ReadNullableBoolean();
              continue;
            default:
              continue;
          }
        }
        else
        {
          geoDistanceQuery.Field = (Field) segment.Utf8String();
          geoDistanceQuery.Location = formatterResolver.GetFormatter<GeoLocation>().Deserialize(ref reader, formatterResolver);
        }
      }
      return (IGeoDistanceQuery) geoDistanceQuery;
    }

    public void Serialize(
      ref JsonWriter writer,
      IGeoDistanceQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        bool flag = false;
        writer.WriteBeginObject();
        if (!value.Name.IsNullOrEmpty())
        {
          writer.WritePropertyName("_name");
          writer.WriteString(value.Name);
          flag = true;
        }
        double? boost = value.Boost;
        if (boost.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("boost");
          ref JsonWriter local = ref writer;
          boost = value.Boost;
          double num = boost.Value;
          local.WriteDouble(num);
          flag = true;
        }
        GeoValidationMethod? validationMethod = value.ValidationMethod;
        if (validationMethod.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("validation_method");
          IJsonFormatter<GeoValidationMethod> formatter = formatterResolver.GetFormatter<GeoValidationMethod>();
          ref JsonWriter local = ref writer;
          validationMethod = value.ValidationMethod;
          int num = (int) validationMethod.Value;
          IJsonFormatterResolver formatterResolver1 = formatterResolver;
          formatter.Serialize(ref local, (GeoValidationMethod) num, formatterResolver1);
          flag = true;
        }
        if (value.Distance != null)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("distance");
          formatterResolver.GetFormatter<Distance>().Serialize(ref writer, value.Distance, formatterResolver);
          flag = true;
        }
        GeoDistanceType? distanceType = value.DistanceType;
        if (distanceType.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("distance_type");
          IJsonFormatter<GeoDistanceType> formatter = formatterResolver.GetFormatter<GeoDistanceType>();
          ref JsonWriter local = ref writer;
          distanceType = value.DistanceType;
          int num = (int) distanceType.Value;
          IJsonFormatterResolver formatterResolver2 = formatterResolver;
          formatter.Serialize(ref local, (GeoDistanceType) num, formatterResolver2);
          flag = true;
        }
        bool? ignoreUnmapped = value.IgnoreUnmapped;
        if (ignoreUnmapped.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("ignore_unmapped");
          ref JsonWriter local = ref writer;
          ignoreUnmapped = value.IgnoreUnmapped;
          int num = ignoreUnmapped.Value ? 1 : 0;
          local.WriteBoolean(num != 0);
          flag = true;
        }
        if (flag)
          writer.WriteValueSeparator();
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        writer.WritePropertyName(connectionSettings.Inferrer.Field(value.Field));
        formatterResolver.GetFormatter<GeoLocation>().Serialize(ref writer, value.Location, formatterResolver);
        writer.WriteEndObject();
      }
    }
  }
}
