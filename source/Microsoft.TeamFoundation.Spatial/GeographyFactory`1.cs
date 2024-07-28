// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyFactory`1
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public class GeographyFactory<T> : SpatialFactory where T : Geography
  {
    private IGeographyProvider provider;
    private GeographyPipeline buildChain;

    internal GeographyFactory(CoordinateSystem coordinateSystem)
    {
      SpatialBuilder destination = SpatialBuilder.Create();
      this.provider = (IGeographyProvider) destination;
      this.buildChain = (GeographyPipeline) SpatialValidator.Create().ChainTo((SpatialPipeline) destination).StartingLink;
      this.buildChain.SetCoordinateSystem(coordinateSystem);
    }

    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public static implicit operator T(GeographyFactory<T> factory) => factory != null ? factory.Build() : throw new ArgumentNullException(nameof (factory));

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeographyFactory<T> Point(double latitude, double longitude, double? z, double? m)
    {
      this.BeginGeo(SpatialType.Point);
      this.LineTo(latitude, longitude, z, m);
      return this;
    }

    public GeographyFactory<T> Point(double latitude, double longitude) => this.Point(latitude, longitude, new double?(), new double?());

    public GeographyFactory<T> Point()
    {
      this.BeginGeo(SpatialType.Point);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeographyFactory<T> LineString(double latitude, double longitude, double? z, double? m)
    {
      this.BeginGeo(SpatialType.LineString);
      this.LineTo(latitude, longitude, z, m);
      return this;
    }

    public GeographyFactory<T> LineString(double latitude, double longitude) => this.LineString(latitude, longitude, new double?(), new double?());

    public GeographyFactory<T> LineString()
    {
      this.BeginGeo(SpatialType.LineString);
      return this;
    }

    public GeographyFactory<T> Polygon()
    {
      this.BeginGeo(SpatialType.Polygon);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public GeographyFactory<T> MultiPoint()
    {
      this.BeginGeo(SpatialType.MultiPoint);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public GeographyFactory<T> MultiLineString()
    {
      this.BeginGeo(SpatialType.MultiLineString);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public GeographyFactory<T> MultiPolygon()
    {
      this.BeginGeo(SpatialType.MultiPolygon);
      return this;
    }

    public GeographyFactory<T> Collection()
    {
      this.BeginGeo(SpatialType.Collection);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeographyFactory<T> Ring(double latitude, double longitude, double? z, double? m)
    {
      this.StartRing(latitude, longitude, z, m);
      return this;
    }

    public GeographyFactory<T> Ring(double latitude, double longitude) => this.Ring(latitude, longitude, new double?(), new double?());

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeographyFactory<T> LineTo(double latitude, double longitude, double? z, double? m)
    {
      this.AddPos(latitude, longitude, z, m);
      return this;
    }

    public GeographyFactory<T> LineTo(double latitude, double longitude) => this.LineTo(latitude, longitude, new double?(), new double?());

    public T Build()
    {
      this.Finish();
      return (T) this.provider.ConstructedGeography;
    }

    protected override void BeginGeo(SpatialType type)
    {
      base.BeginGeo(type);
      this.buildChain.BeginGeography(type);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    protected override void BeginFigure(double latitude, double longitude, double? z, double? m)
    {
      base.BeginFigure(latitude, longitude, z, m);
      this.buildChain.BeginFigure(new GeographyPosition(latitude, longitude, z, m));
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    protected override void AddLine(double latitude, double longitude, double? z, double? m)
    {
      base.AddLine(latitude, longitude, z, m);
      this.buildChain.LineTo(new GeographyPosition(latitude, longitude, z, m));
    }

    protected override void EndFigure()
    {
      base.EndFigure();
      this.buildChain.EndFigure();
    }

    protected override void EndGeo()
    {
      base.EndGeo();
      this.buildChain.EndGeography();
    }
  }
}
