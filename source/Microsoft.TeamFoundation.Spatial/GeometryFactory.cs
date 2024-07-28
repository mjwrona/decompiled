// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryFactory
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public static class GeometryFactory
  {
    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryPoint> Point(
      CoordinateSystem coordinateSystem,
      double x,
      double y,
      double? z,
      double? m)
    {
      return new GeometryFactory<GeometryPoint>(coordinateSystem).Point(x, y, z, m);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryPoint> Point(double x, double y, double? z, double? m) => GeometryFactory.Point(CoordinateSystem.DefaultGeometry, x, y, z, m);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryPoint> Point(
      CoordinateSystem coordinateSystem,
      double x,
      double y)
    {
      return GeometryFactory.Point(coordinateSystem, x, y, new double?(), new double?());
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryPoint> Point(double x, double y) => GeometryFactory.Point(CoordinateSystem.DefaultGeometry, x, y, new double?(), new double?());

    public static GeometryFactory<GeometryPoint> Point(CoordinateSystem coordinateSystem) => new GeometryFactory<GeometryPoint>(coordinateSystem).Point();

    public static GeometryFactory<GeometryPoint> Point() => GeometryFactory.Point(CoordinateSystem.DefaultGeometry);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeometryFactory<GeometryMultiPoint> MultiPoint(CoordinateSystem coordinateSystem) => new GeometryFactory<GeometryMultiPoint>(coordinateSystem).MultiPoint();

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeometryFactory<GeometryMultiPoint> MultiPoint() => GeometryFactory.MultiPoint(CoordinateSystem.DefaultGeometry);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryLineString> LineString(
      CoordinateSystem coordinateSystem,
      double x,
      double y,
      double? z,
      double? m)
    {
      return new GeometryFactory<GeometryLineString>(coordinateSystem).LineString(x, y, z, m);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryLineString> LineString(
      double x,
      double y,
      double? z,
      double? m)
    {
      return GeometryFactory.LineString(CoordinateSystem.DefaultGeometry, x, y, z, m);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryLineString> LineString(
      CoordinateSystem coordinateSystem,
      double x,
      double y)
    {
      return GeometryFactory.LineString(coordinateSystem, x, y, new double?(), new double?());
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryFactory<GeometryLineString> LineString(double x, double y) => GeometryFactory.LineString(CoordinateSystem.DefaultGeometry, x, y, new double?(), new double?());

    public static GeometryFactory<GeometryLineString> LineString(CoordinateSystem coordinateSystem) => new GeometryFactory<GeometryLineString>(coordinateSystem).LineString();

    public static GeometryFactory<GeometryLineString> LineString() => GeometryFactory.LineString(CoordinateSystem.DefaultGeometry);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeometryFactory<GeometryMultiLineString> MultiLineString(
      CoordinateSystem coordinateSystem)
    {
      return new GeometryFactory<GeometryMultiLineString>(coordinateSystem).MultiLineString();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeometryFactory<GeometryMultiLineString> MultiLineString() => GeometryFactory.MultiLineString(CoordinateSystem.DefaultGeometry);

    public static GeometryFactory<GeometryPolygon> Polygon(CoordinateSystem coordinateSystem) => new GeometryFactory<GeometryPolygon>(coordinateSystem).Polygon();

    public static GeometryFactory<GeometryPolygon> Polygon() => GeometryFactory.Polygon(CoordinateSystem.DefaultGeometry);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeometryFactory<GeometryMultiPolygon> MultiPolygon(
      CoordinateSystem coordinateSystem)
    {
      return new GeometryFactory<GeometryMultiPolygon>(coordinateSystem).MultiPolygon();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public static GeometryFactory<GeometryMultiPolygon> MultiPolygon() => GeometryFactory.MultiPolygon(CoordinateSystem.DefaultGeometry);

    public static GeometryFactory<GeometryCollection> Collection(CoordinateSystem coordinateSystem) => new GeometryFactory<GeometryCollection>(coordinateSystem).Collection();

    public static GeometryFactory<GeometryCollection> Collection() => GeometryFactory.Collection(CoordinateSystem.DefaultGeometry);
  }
}
