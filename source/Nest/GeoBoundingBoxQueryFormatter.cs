// Decompiled with JetBrains decompiler
// Type: Nest.GeoBoundingBoxQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class GeoBoundingBoxQueryFormatter : IJsonFormatter<IGeoBoundingBoxQuery>, IJsonFormatter
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
        "type",
        3
      },
      {
        "ignore_unmapped",
        4
      }
    };

    public IGeoBoundingBoxQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IGeoBoundingBoxQuery) null;
      GeoBoundingBoxQuery boundingBoxQuery = new GeoBoundingBoxQuery();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (GeoBoundingBoxQueryFormatter.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              boundingBoxQuery.Name = reader.ReadString();
              continue;
            case 1:
              boundingBoxQuery.Boost = new double?(reader.ReadDouble());
              continue;
            case 2:
              boundingBoxQuery.ValidationMethod = new GeoValidationMethod?(formatterResolver.GetFormatter<GeoValidationMethod>().Deserialize(ref reader, formatterResolver));
              continue;
            case 3:
              boundingBoxQuery.Type = new GeoExecution?(formatterResolver.GetFormatter<GeoExecution>().Deserialize(ref reader, formatterResolver));
              continue;
            case 4:
              boundingBoxQuery.IgnoreUnmapped = reader.ReadNullableBoolean();
              continue;
            default:
              continue;
          }
        }
        else
        {
          boundingBoxQuery.Field = (Field) segment.Utf8String();
          boundingBoxQuery.BoundingBox = formatterResolver.GetFormatter<IBoundingBox>().Deserialize(ref reader, formatterResolver);
        }
      }
      return (IGeoBoundingBoxQuery) boundingBoxQuery;
    }

    public void Serialize(
      ref JsonWriter writer,
      IGeoBoundingBoxQuery value,
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
        GeoExecution? type = value.Type;
        if (type.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("type");
          IJsonFormatter<GeoExecution> formatter = formatterResolver.GetFormatter<GeoExecution>();
          ref JsonWriter local = ref writer;
          type = value.Type;
          int num = (int) type.Value;
          IJsonFormatterResolver formatterResolver2 = formatterResolver;
          formatter.Serialize(ref local, (GeoExecution) num, formatterResolver2);
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
        formatterResolver.GetFormatter<IBoundingBox>().Serialize(ref writer, value.BoundingBox, formatterResolver);
        writer.WriteEndObject();
      }
    }
  }
}
