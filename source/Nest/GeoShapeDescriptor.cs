// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GeoShapeDescriptor : DescriptorBase<GeoShapeDescriptor, IDescriptor>
  {
    public IGeoShape Circle(GeoCoordinate coordinate, string radius) => (IGeoShape) new CircleGeoShape(coordinate, radius);

    public IGeoShape Envelope(GeoCoordinate topLeftCoordinate, GeoCoordinate bottomRightCoordinate) => (IGeoShape) new EnvelopeGeoShape((IEnumerable<GeoCoordinate>) new GeoCoordinate[2]
    {
      topLeftCoordinate,
      bottomRightCoordinate
    });

    public IGeoShape Envelope(IEnumerable<GeoCoordinate> coordinates) => (IGeoShape) new EnvelopeGeoShape(coordinates);

    public IGeoShape GeometryCollection(IEnumerable<IGeoShape> geometries) => (IGeoShape) new Nest.GeometryCollection(geometries);

    public IGeoShape GeometryCollection(params IGeoShape[] geometries) => (IGeoShape) new Nest.GeometryCollection((IEnumerable<IGeoShape>) geometries);

    public IGeoShape LineString(IEnumerable<GeoCoordinate> coordinates) => (IGeoShape) new LineStringGeoShape(coordinates);

    public IGeoShape LineString(params GeoCoordinate[] coordinates) => (IGeoShape) new LineStringGeoShape((IEnumerable<GeoCoordinate>) coordinates);

    public IGeoShape MultiLineString(
      IEnumerable<IEnumerable<GeoCoordinate>> coordinates)
    {
      return (IGeoShape) new MultiLineStringGeoShape(coordinates);
    }

    public IGeoShape MultiLineString(params IEnumerable<GeoCoordinate>[] coordinates) => (IGeoShape) new MultiLineStringGeoShape((IEnumerable<IEnumerable<GeoCoordinate>>) coordinates);

    public IGeoShape Point(GeoCoordinate coordinates) => (IGeoShape) new PointGeoShape(coordinates);

    public IGeoShape MultiPoint(IEnumerable<GeoCoordinate> coordinates) => (IGeoShape) new MultiPointGeoShape(coordinates);

    public IGeoShape MultiPoint(params GeoCoordinate[] coordinates) => (IGeoShape) new MultiPointGeoShape((IEnumerable<GeoCoordinate>) coordinates);

    public IGeoShape Polygon(
      IEnumerable<IEnumerable<GeoCoordinate>> coordinates)
    {
      return (IGeoShape) new PolygonGeoShape(coordinates);
    }

    public IGeoShape Polygon(params IEnumerable<GeoCoordinate>[] coordinates) => (IGeoShape) new PolygonGeoShape((IEnumerable<IEnumerable<GeoCoordinate>>) coordinates);

    public IGeoShape MultiPolygon(
      IEnumerable<IEnumerable<IEnumerable<GeoCoordinate>>> coordinates)
    {
      return (IGeoShape) new MultiPolygonGeoShape(coordinates);
    }

    public IGeoShape MultiPolygon(
      params IEnumerable<IEnumerable<GeoCoordinate>>[] coordinates)
    {
      return (IGeoShape) new MultiPolygonGeoShape((IEnumerable<IEnumerable<IEnumerable<GeoCoordinate>>>) coordinates);
    }
  }
}
