// Decompiled with JetBrains decompiler
// Type: Nest.GeoWKTWriter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nest
{
  public class GeoWKTWriter
  {
    public static string Write(IGeoShape shape) => shape != null ? GeoWKTWriter.Write(shape, new StringBuilder()) : (string) null;

    private static string Write(IGeoShape shape, StringBuilder builder)
    {
      switch (shape)
      {
        case IPointGeoShape point:
          GeoWKTWriter.WritePoint(point, builder);
          break;
        case IMultiPointGeoShape multiPoint:
          GeoWKTWriter.WriteMultiPoint(multiPoint, builder);
          break;
        case ILineStringGeoShape lineString:
          GeoWKTWriter.WriteLineString(lineString, builder);
          break;
        case IMultiLineStringGeoShape multiLineString:
          GeoWKTWriter.WriteMultiLineString(multiLineString, builder);
          break;
        case IPolygonGeoShape polygon:
          GeoWKTWriter.WritePolygon(polygon, builder);
          break;
        case IMultiPolygonGeoShape multiPolygon:
          GeoWKTWriter.WriteMultiPolygon(multiPolygon, builder);
          break;
        case IGeometryCollection geometryCollection:
          GeoWKTWriter.WriteGeometryCollection(geometryCollection, builder);
          break;
        case IEnvelopeGeoShape envelope:
          GeoWKTWriter.WriteEnvelope(envelope, builder);
          break;
        default:
          throw new GeoWKTException("Unknown geometry type: " + shape.GetType().Name);
      }
      return builder.ToString();
    }

    private static void WritePoint(IPointGeoShape point, StringBuilder builder)
    {
      builder.Append("POINT").Append(" (");
      GeoWKTWriter.WriteCoordinate(point.Coordinates, builder);
      builder.Append(")");
    }

    private static void WriteMultiPoint(IMultiPointGeoShape multiPoint, StringBuilder builder)
    {
      builder.Append("MULTIPOINT").Append(" (");
      GeoWKTWriter.WriteCoordinates(multiPoint.Coordinates, builder);
      builder.Append(")");
    }

    private static void WriteLineString(ILineStringGeoShape lineString, StringBuilder builder)
    {
      builder.Append("LINESTRING").Append(" (");
      GeoWKTWriter.WriteCoordinates(lineString.Coordinates, builder);
      builder.Append(")");
    }

    private static void WriteMultiLineString(
      IMultiLineStringGeoShape multiLineString,
      StringBuilder builder)
    {
      builder.Append("MULTILINESTRING").Append(" ");
      GeoWKTWriter.WriteCoordinatesList(multiLineString.Coordinates, builder);
    }

    private static void WritePolygon(IPolygonGeoShape polygon, StringBuilder builder)
    {
      builder.Append("POLYGON").Append(" ");
      GeoWKTWriter.WriteCoordinatesList(polygon.Coordinates, builder);
    }

    private static void WriteMultiPolygon(IMultiPolygonGeoShape multiPolygon, StringBuilder builder)
    {
      builder.Append("MULTIPOLYGON").Append(" (");
      int num = 0;
      foreach (IEnumerable<IEnumerable<GeoCoordinate>> coordinate in multiPolygon.Coordinates)
      {
        if (num > 0)
          builder.Append(", ");
        GeoWKTWriter.WriteCoordinatesList(coordinate, builder);
        ++num;
      }
      builder.Append(")");
    }

    private static void WriteGeometryCollection(
      IGeometryCollection geometryCollection,
      StringBuilder builder)
    {
      builder.Append("GEOMETRYCOLLECTION").Append(" (");
      int num = 0;
      foreach (IGeoShape geometry in geometryCollection.Geometries)
      {
        if (num > 0)
          builder.Append(", ");
        GeoWKTWriter.Write(geometry, builder);
        ++num;
      }
      builder.Append(")");
    }

    private static void WriteEnvelope(IEnvelopeGeoShape envelope, StringBuilder builder)
    {
      builder.Append("BBOX").Append(" (");
      GeoCoordinate geoCoordinate1 = envelope.Coordinates.ElementAt<GeoCoordinate>(0);
      GeoCoordinate geoCoordinate2 = envelope.Coordinates.ElementAt<GeoCoordinate>(1);
      builder.Append(geoCoordinate1.Longitude.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append(", ").Append(geoCoordinate2.Longitude.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append(", ").Append(geoCoordinate1.Latitude.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append(", ").Append(geoCoordinate2.Latitude.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append(")");
    }

    private static void WriteCoordinatesList(
      IEnumerable<IEnumerable<GeoCoordinate>> coordinates,
      StringBuilder builder)
    {
      builder.Append("(");
      int num = 0;
      foreach (IEnumerable<GeoCoordinate> coordinate in coordinates)
      {
        if (num > 0)
          builder.Append(", ");
        builder.Append("(");
        GeoWKTWriter.WriteCoordinates(coordinate, builder);
        builder.Append(")");
        ++num;
      }
      builder.Append(")");
    }

    private static void WriteCoordinates(
      IEnumerable<GeoCoordinate> coordinates,
      StringBuilder builder)
    {
      int num = 0;
      foreach (GeoCoordinate coordinate in coordinates)
      {
        if (num > 0)
          builder.Append(", ");
        GeoWKTWriter.WriteCoordinate(coordinate, builder);
        ++num;
      }
    }

    private static void WriteCoordinate(GeoCoordinate coordinate, StringBuilder builder)
    {
      builder.Append(coordinate.Longitude.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append(" ").Append(coordinate.Latitude.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!coordinate.Z.HasValue)
        return;
      builder.Append(" ").Append(coordinate.Z.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
