// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeQueryFieldNameFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class GeoShapeQueryFieldNameFormatter : IJsonFormatter<IGeoShapeQuery>, IJsonFormatter
  {
    public IGeoShapeQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    public void Serialize(
      ref JsonWriter writer,
      IGeoShapeQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      Field field = value.Field;
      if (field == (Field) null)
      {
        writer.WriteNull();
      }
      else
      {
        string propertyName = formatterResolver.GetConnectionSettings().Inferrer.Field(field);
        if (propertyName.IsNullOrEmpty())
        {
          writer.WriteNull();
        }
        else
        {
          writer.WriteBeginObject();
          string name = value.Name;
          double? boost = value.Boost;
          bool? ignoreUnmapped = value.IgnoreUnmapped;
          if (!name.IsNullOrEmpty())
          {
            writer.WritePropertyName("_name");
            writer.WriteString(name);
            writer.WriteValueSeparator();
          }
          if (boost.HasValue)
          {
            writer.WritePropertyName("boost");
            writer.WriteDouble(boost.Value);
            writer.WriteValueSeparator();
          }
          if (ignoreUnmapped.HasValue)
          {
            writer.WritePropertyName("ignore_unmapped");
            writer.WriteBoolean(ignoreUnmapped.Value);
            writer.WriteValueSeparator();
          }
          writer.WritePropertyName(propertyName);
          writer.WriteBeginObject();
          bool flag = false;
          if (value.Shape != null)
          {
            writer.WritePropertyName("shape");
            formatterResolver.GetFormatter<IGeoShape>().Serialize(ref writer, value.Shape, formatterResolver);
            flag = true;
          }
          else if (value.IndexedShape != null)
          {
            writer.WritePropertyName("indexed_shape");
            formatterResolver.GetFormatter<IFieldLookup>().Serialize(ref writer, value.IndexedShape, formatterResolver);
            flag = true;
          }
          GeoShapeRelation? relation = value.Relation;
          if (relation.HasValue)
          {
            if (flag)
              writer.WriteValueSeparator();
            writer.WritePropertyName("relation");
            IJsonFormatter<GeoShapeRelation> formatter = formatterResolver.GetFormatter<GeoShapeRelation>();
            ref JsonWriter local = ref writer;
            relation = value.Relation;
            int num = (int) relation.Value;
            IJsonFormatterResolver formatterResolver1 = formatterResolver;
            formatter.Serialize(ref local, (GeoShapeRelation) num, formatterResolver1);
          }
          writer.WriteEndObject();
          writer.WriteEndObject();
        }
      }
    }
  }
}
