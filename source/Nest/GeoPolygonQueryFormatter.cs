// Decompiled with JetBrains decompiler
// Type: Nest.GeoPolygonQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class GeoPolygonQueryFormatter : IJsonFormatter<IGeoPolygonQuery>, IJsonFormatter
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
        "ignore_unmapped",
        3
      }
    };

    public IGeoPolygonQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IGeoPolygonQuery) null;
      GeoPolygonQuery geoPolygonQuery = new GeoPolygonQuery();
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (GeoPolygonQueryFormatter.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              geoPolygonQuery.Name = reader.ReadString();
              continue;
            case 1:
              geoPolygonQuery.Boost = new double?(reader.ReadDouble());
              continue;
            case 2:
              geoPolygonQuery.ValidationMethod = new GeoValidationMethod?(formatterResolver.GetFormatter<GeoValidationMethod>().Deserialize(ref reader, formatterResolver));
              continue;
            case 3:
              geoPolygonQuery.IgnoreUnmapped = reader.ReadNullableBoolean();
              continue;
            default:
              continue;
          }
        }
        else
        {
          geoPolygonQuery.Field = (Field) segment.Utf8String();
          int count2 = 0;
          while (reader.ReadIsInObject(ref count2))
          {
            reader.ReadNext();
            reader.ReadIsNameSeparatorWithVerify();
            geoPolygonQuery.Points = formatterResolver.GetFormatter<IEnumerable<GeoLocation>>().Deserialize(ref reader, formatterResolver);
          }
        }
      }
      return (IGeoPolygonQuery) geoPolygonQuery;
    }

    public void Serialize(
      ref JsonWriter writer,
      IGeoPolygonQuery value,
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
        writer.WriteBeginObject();
        writer.WritePropertyName("points");
        formatterResolver.GetFormatter<IEnumerable<GeoLocation>>().Serialize(ref writer, value.Points, formatterResolver);
        writer.WriteEndObject();
        writer.WriteEndObject();
      }
    }
  }
}
