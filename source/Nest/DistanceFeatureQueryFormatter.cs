// Decompiled with JetBrains decompiler
// Type: Nest.DistanceFeatureQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;

namespace Nest
{
  internal class DistanceFeatureQueryFormatter : 
    IJsonFormatter<IDistanceFeatureQuery>,
    IJsonFormatter
  {
    private static readonly UnionFormatter<GeoCoordinate, DateMath> OriginUnionFormatter = new UnionFormatter<GeoCoordinate, DateMath>(true);
    private static readonly UnionFormatter<Distance, Time> PivotUnionFormatter = new UnionFormatter<Distance, Time>();
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "field",
        0
      },
      {
        "origin",
        1
      },
      {
        "pivot",
        2
      },
      {
        "boost",
        3
      },
      {
        "_name",
        4
      }
    };

    public void Serialize(
      ref JsonWriter writer,
      IDistanceFeatureQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        if (!string.IsNullOrEmpty(value.Name))
        {
          writer.WritePropertyName("_name");
          writer.WriteString(value.Name);
          writer.WriteValueSeparator();
        }
        double? boost = value.Boost;
        if (boost.HasValue)
        {
          writer.WritePropertyName("boost");
          ref JsonWriter local = ref writer;
          boost = value.Boost;
          double num = boost.Value;
          local.WriteDouble(num);
          writer.WriteValueSeparator();
        }
        writer.WritePropertyName("field");
        formatterResolver.GetFormatter<Field>().Serialize(ref writer, value.Field, formatterResolver);
        writer.WriteValueSeparator();
        writer.WritePropertyName("origin");
        DistanceFeatureQueryFormatter.OriginUnionFormatter.Serialize(ref writer, value.Origin, formatterResolver);
        writer.WriteValueSeparator();
        writer.WritePropertyName("pivot");
        DistanceFeatureQueryFormatter.PivotUnionFormatter.Serialize(ref writer, value.Pivot, formatterResolver);
        writer.WriteEndObject();
      }
    }

    public IDistanceFeatureQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IDistanceFeatureQuery) null;
      DistanceFeatureQuery distanceFeatureQuery = new DistanceFeatureQuery();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        int num;
        if (DistanceFeatureQueryFormatter.Fields.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
        {
          switch (num)
          {
            case 0:
              distanceFeatureQuery.Field = formatterResolver.GetFormatter<Field>().Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              distanceFeatureQuery.Origin = DistanceFeatureQueryFormatter.OriginUnionFormatter.Deserialize(ref reader, formatterResolver);
              continue;
            case 2:
              distanceFeatureQuery.Pivot = DistanceFeatureQueryFormatter.PivotUnionFormatter.Deserialize(ref reader, formatterResolver);
              continue;
            case 3:
              distanceFeatureQuery.Boost = new double?(reader.ReadDouble());
              continue;
            case 4:
              distanceFeatureQuery.Name = reader.ReadString();
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      return (IDistanceFeatureQuery) distanceFeatureQuery;
    }
  }
}
