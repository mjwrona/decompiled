// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class GeoShapeFormatter : IJsonFormatter<IGeoShape>, IJsonFormatter
  {
    private static readonly AutomataDictionary CircleFields = new AutomataDictionary()
    {
      {
        "coordinates",
        0
      },
      {
        "radius",
        1
      }
    };
    private static readonly byte[] CoordinatesField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("coordinates");
    private static readonly byte[] GeometriesField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("geometries");
    internal static readonly GeoShapeFormatter Instance = new GeoShapeFormatter();
    private static readonly byte[] TypeField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("type");

    public IGeoShape Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.String:
          return GeoWKTReader.Read(reader.ReadString());
        case JsonToken.Null:
          reader.ReadNext();
          return (IGeoShape) null;
        default:
          return GeoShapeFormatter.ReadShape(ref reader, formatterResolver);
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IGeoShape value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else if (value is GeoShapeBase shape && shape.Format == GeoFormat.WellKnownText)
      {
        writer.WriteString(GeoWKTWriter.Write((IGeoShape) shape));
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName("type");
        writer.WriteString(value.Type);
        writer.WriteValueSeparator();
        if (!(value is IPointGeoShape pointGeoShape))
        {
          if (!(value is IMultiPointGeoShape multiPointGeoShape))
          {
            if (!(value is ILineStringGeoShape lineStringGeoShape2))
            {
              if (!(value is IMultiLineStringGeoShape lineStringGeoShape1))
              {
                if (!(value is IPolygonGeoShape polygonGeoShape))
                {
                  if (!(value is IMultiPolygonGeoShape multiPolygonGeoShape))
                  {
                    if (!(value is IEnvelopeGeoShape envelopeGeoShape))
                    {
                      if (!(value is ICircleGeoShape circleGeoShape))
                      {
                        if (value is IGeometryCollection geometryCollection)
                        {
                          writer.WritePropertyName("geometries");
                          formatterResolver.GetFormatter<IEnumerable<IGeoShape>>().Serialize(ref writer, geometryCollection.Geometries, formatterResolver);
                        }
                      }
                      else
                      {
                        writer.WritePropertyName("coordinates");
                        formatterResolver.GetFormatter<GeoCoordinate>().Serialize(ref writer, circleGeoShape.Coordinates, formatterResolver);
                        writer.WriteValueSeparator();
                        writer.WritePropertyName("radius");
                        writer.WriteString(circleGeoShape.Radius);
                      }
                    }
                    else
                    {
                      writer.WritePropertyName("coordinates");
                      formatterResolver.GetFormatter<IEnumerable<GeoCoordinate>>().Serialize(ref writer, envelopeGeoShape.Coordinates, formatterResolver);
                    }
                  }
                  else
                  {
                    writer.WritePropertyName("coordinates");
                    formatterResolver.GetFormatter<IEnumerable<IEnumerable<IEnumerable<GeoCoordinate>>>>().Serialize(ref writer, multiPolygonGeoShape.Coordinates, formatterResolver);
                  }
                }
                else
                {
                  writer.WritePropertyName("coordinates");
                  formatterResolver.GetFormatter<IEnumerable<IEnumerable<GeoCoordinate>>>().Serialize(ref writer, polygonGeoShape.Coordinates, formatterResolver);
                }
              }
              else
              {
                writer.WritePropertyName("coordinates");
                formatterResolver.GetFormatter<IEnumerable<IEnumerable<GeoCoordinate>>>().Serialize(ref writer, lineStringGeoShape1.Coordinates, formatterResolver);
              }
            }
            else
            {
              writer.WritePropertyName("coordinates");
              formatterResolver.GetFormatter<IEnumerable<GeoCoordinate>>().Serialize(ref writer, lineStringGeoShape2.Coordinates, formatterResolver);
            }
          }
          else
          {
            writer.WritePropertyName("coordinates");
            formatterResolver.GetFormatter<IEnumerable<GeoCoordinate>>().Serialize(ref writer, multiPointGeoShape.Coordinates, formatterResolver);
          }
        }
        else
        {
          writer.WritePropertyName("coordinates");
          formatterResolver.GetFormatter<GeoCoordinate>().Serialize(ref writer, pointGeoShape.Coordinates, formatterResolver);
        }
        writer.WriteEndObject();
      }
    }

    internal static IGeoShape ReadShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment1 = reader.ReadNextBlockSegment();
      int count = 0;
      JsonReader reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      string str = (string) null;
      while (reader1.ReadIsInObject(ref count))
      {
        ArraySegment<byte> arraySegment2 = reader1.ReadPropertyNameSegmentRaw();
        if (arraySegment2.EqualsBytes(GeoShapeFormatter.TypeField))
        {
          str = reader1.ReadString().ToUpperInvariant();
          break;
        }
        reader1.ReadNextBlock();
      }
      reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      switch (str)
      {
        case "CIRCLE":
          return (IGeoShape) GeoShapeFormatter.ParseCircleGeoShape(ref reader1, formatterResolver);
        case "ENVELOPE":
          return (IGeoShape) GeoShapeFormatter.ParseEnvelopeGeoShape(ref reader1, formatterResolver);
        case "GEOMETRYCOLLECTION":
          return (IGeoShape) GeoShapeFormatter.ParseGeometryCollection(ref reader1, formatterResolver);
        case "LINESTRING":
          return (IGeoShape) GeoShapeFormatter.ParseLineStringGeoShape(ref reader1, formatterResolver);
        case "MULTILINESTRING":
          return (IGeoShape) GeoShapeFormatter.ParseMultiLineStringGeoShape(ref reader1, formatterResolver);
        case "MULTIPOINT":
          return (IGeoShape) GeoShapeFormatter.ParseMultiPointGeoShape(ref reader1, formatterResolver);
        case "MULTIPOLYGON":
          return (IGeoShape) GeoShapeFormatter.ParseMultiPolygonGeoShape(ref reader1, formatterResolver);
        case "POINT":
          return (IGeoShape) GeoShapeFormatter.ParsePointGeoShape(ref reader1, formatterResolver);
        case "POLYGON":
          return (IGeoShape) GeoShapeFormatter.ParsePolygonGeoShape(ref reader1, formatterResolver);
        default:
          return (IGeoShape) null;
      }
    }

    private static GeometryCollection ParseGeometryCollection(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      int count = 0;
      IEnumerable<IGeoShape> geoShapes = Enumerable.Empty<IGeoShape>();
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
        if (arraySegment.EqualsBytes(GeoShapeFormatter.GeometriesField))
        {
          geoShapes = formatterResolver.GetFormatter<IEnumerable<IGeoShape>>().Deserialize(ref reader, formatterResolver);
          break;
        }
        reader.ReadNextBlock();
      }
      return new GeometryCollection()
      {
        Geometries = geoShapes
      };
    }

    private static MultiPolygonGeoShape ParseMultiPolygonGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new MultiPolygonGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<IEnumerable<IEnumerable<IEnumerable<GeoCoordinate>>>>(ref reader, formatterResolver)
      };
    }

    private static PolygonGeoShape ParsePolygonGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new PolygonGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<IEnumerable<IEnumerable<GeoCoordinate>>>(ref reader, formatterResolver)
      };
    }

    private static MultiPointGeoShape ParseMultiPointGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new MultiPointGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<IEnumerable<GeoCoordinate>>(ref reader, formatterResolver)
      };
    }

    private static PointGeoShape ParsePointGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new PointGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<GeoCoordinate>(ref reader, formatterResolver)
      };
    }

    private static MultiLineStringGeoShape ParseMultiLineStringGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new MultiLineStringGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<IEnumerable<IEnumerable<GeoCoordinate>>>(ref reader, formatterResolver)
      };
    }

    private static LineStringGeoShape ParseLineStringGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new LineStringGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<IEnumerable<GeoCoordinate>>(ref reader, formatterResolver)
      };
    }

    private static EnvelopeGeoShape ParseEnvelopeGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new EnvelopeGeoShape()
      {
        Coordinates = GeoShapeFormatter.GetCoordinates<IEnumerable<GeoCoordinate>>(ref reader, formatterResolver)
      };
    }

    private static CircleGeoShape ParseCircleGeoShape(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      int count = 0;
      string str = (string) null;
      GeoCoordinate geoCoordinate = (GeoCoordinate) null;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (GeoShapeFormatter.CircleFields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              geoCoordinate = formatterResolver.GetFormatter<GeoCoordinate>().Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              str = reader.ReadString();
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      return new CircleGeoShape()
      {
        Coordinates = geoCoordinate,
        Radius = str
      };
    }

    private static T GetCoordinates<T>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      int count = 0;
      T coordinates = default (T);
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
        if (arraySegment.EqualsBytes(GeoShapeFormatter.CoordinatesField))
        {
          coordinates = formatterResolver.GetFormatter<T>().Deserialize(ref reader, formatterResolver);
          break;
        }
        reader.ReadNextBlock();
      }
      return coordinates;
    }
  }
}
