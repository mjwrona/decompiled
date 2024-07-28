// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyFactory
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public static class GeographyFactory
  {
    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeographyFactory<GeographyPoint> Point(
      CoordinateSystem coordinateSystem,
      double latitude,
      double longitude,
      double? z,
      double? m)
    {
      return new GeographyFactory<GeographyPoint>(coordinateSystem).Point(latitude, longitude, z, m);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeographyFactory<GeographyPoint> Point(
      double latitude,
      double longitude,
      double? z,
      double? m)
    {
      return GeographyFactory.Point(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);
    }

    public static GeographyFactory<GeographyPoint> Point(
      CoordinateSystem coordinateSystem,
      double latitude,
      double longitude)
    {
      return GeographyFactory.Point(coordinateSystem, latitude, longitude, new double?(), new double?());
    }

    public static GeographyFactory<GeographyPoint> Point(double latitude, double longitude) => GeographyFactory.Point(CoordinateSystem.DefaultGeography, latitude, longitude, new double?(), new double?());

    public static GeographyFactory<GeographyPoint> Point(CoordinateSystem coordinateSystem) => new GeographyFactory<GeographyPoint>(coordinateSystem).Point();

    public static GeographyFactory<GeographyPoint> Point() => GeographyFactory.Point(CoordinateSystem.DefaultGeography);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeographyFactory<GeographyMultiPoint> MultiPoint(CoordinateSystem coordinateSystem) => new GeographyFactory<GeographyMultiPoint>(coordinateSystem).MultiPoint();

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeographyFactory<GeographyMultiPoint> MultiPoint() => GeographyFactory.MultiPoint(CoordinateSystem.DefaultGeography);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeographyFactory<GeographyLineString> LineString(
      CoordinateSystem coordinateSystem,
      double latitude,
      double longitude,
      double? z,
      double? m)
    {
      return new GeographyFactory<GeographyLineString>(coordinateSystem).LineString(latitude, longitude, z, m);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeographyFactory<GeographyLineString> LineString(
      double latitude,
      double longitude,
      double? z,
      double? m)
    {
      return GeographyFactory.LineString(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);
    }

    public static GeographyFactory<GeographyLineString> LineString(
      CoordinateSystem coordinateSystem,
      double latitude,
      double longitude)
    {
      return GeographyFactory.LineString(coordinateSystem, latitude, longitude, new double?(), new double?());
    }

    public static GeographyFactory<GeographyLineString> LineString(
      double latitude,
      double longitude)
    {
      return GeographyFactory.LineString(CoordinateSystem.DefaultGeography, latitude, longitude, new double?(), new double?());
    }

    public static GeographyFactory<GeographyLineString> LineString(CoordinateSystem coordinateSystem) => new GeographyFactory<GeographyLineString>(coordinateSystem).LineString();

    public static GeographyFactory<GeographyLineString> LineString() => GeographyFactory.LineString(CoordinateSystem.DefaultGeography);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeographyFactory<GeographyMultiLineString> MultiLineString(
      CoordinateSystem coordinateSystem)
    {
      return new GeographyFactory<GeographyMultiLineString>(coordinateSystem).MultiLineString();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeographyFactory<GeographyMultiLineString> MultiLineString() => GeographyFactory.MultiLineString(CoordinateSystem.DefaultGeography);

    public static GeographyFactory<GeographyPolygon> Polygon(CoordinateSystem coordinateSystem) => new GeographyFactory<GeographyPolygon>(coordinateSystem).Polygon();

    public static GeographyFactory<GeographyPolygon> Polygon() => GeographyFactory.Polygon(CoordinateSystem.DefaultGeography);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeographyFactory<GeographyMultiPolygon> MultiPolygon(
      CoordinateSystem coordinateSystem)
    {
      return new GeographyFactory<GeographyMultiPolygon>(coordinateSystem).MultiPolygon();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeographyFactory<GeographyMultiPolygon> MultiPolygon() => GeographyFactory.MultiPolygon(CoordinateSystem.DefaultGeography);

    public static GeographyFactory<GeographyCollection> Collection(CoordinateSystem coordinateSystem) => new GeographyFactory<GeographyCollection>(coordinateSystem).Collection();

    public static GeographyFactory<GeographyCollection> Collection() => GeographyFactory.Collection(CoordinateSystem.DefaultGeography);
  }
}
