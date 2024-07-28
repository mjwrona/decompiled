// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryFactory`1
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public class GeometryFactory<T> : SpatialFactory where T : Geometry
  {
    private IGeometryProvider provider;
    private GeometryPipeline buildChain;

    internal GeometryFactory(CoordinateSystem coordinateSystem)
    {
      SpatialBuilder destination = SpatialBuilder.Create();
      this.provider = (IGeometryProvider) destination;
      this.buildChain = (GeometryPipeline) SpatialValidator.Create().ChainTo((SpatialPipeline) destination).StartingLink;
      this.buildChain.SetCoordinateSystem(coordinateSystem);
    }

    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Operator used to build")]
    public static implicit operator T(GeometryFactory<T> factory) => factory != null ? factory.Build() : default (T);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> Point(double x, double y, double? z, double? m)
    {
      this.BeginGeo(SpatialType.Point);
      this.LineTo(x, y, z, m);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> Point(double x, double y) => this.Point(x, y, new double?(), new double?());

    public GeometryFactory<T> Point()
    {
      this.BeginGeo(SpatialType.Point);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> LineString(double x, double y, double? z, double? m)
    {
      this.BeginGeo(SpatialType.LineString);
      this.LineTo(x, y, z, m);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> LineString(double x, double y) => this.LineString(x, y, new double?(), new double?());

    public GeometryFactory<T> LineString()
    {
      this.BeginGeo(SpatialType.LineString);
      return this;
    }

    public GeometryFactory<T> Polygon()
    {
      this.BeginGeo(SpatialType.Polygon);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public GeometryFactory<T> MultiPoint()
    {
      this.BeginGeo(SpatialType.MultiPoint);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public GeometryFactory<T> MultiLineString()
    {
      this.BeginGeo(SpatialType.MultiLineString);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
    public GeometryFactory<T> MultiPolygon()
    {
      this.BeginGeo(SpatialType.MultiPolygon);
      return this;
    }

    public GeometryFactory<T> Collection()
    {
      this.BeginGeo(SpatialType.Collection);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> Ring(double x, double y, double? z, double? m)
    {
      this.StartRing(x, y, z, m);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> Ring(double x, double y) => this.Ring(x, y, new double?(), new double?());

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> LineTo(double x, double y, double? z, double? m)
    {
      this.AddPos(x, y, z, m);
      return this;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public GeometryFactory<T> LineTo(double x, double y) => this.LineTo(x, y, new double?(), new double?());

    public T Build()
    {
      this.Finish();
      return (T) this.provider.ConstructedGeometry;
    }

    protected override void BeginGeo(SpatialType type)
    {
      base.BeginGeo(type);
      this.buildChain.BeginGeometry(type);
    }

    protected override void BeginFigure(double x, double y, double? z, double? m)
    {
      base.BeginFigure(x, y, z, m);
      this.buildChain.BeginFigure(new GeometryPosition(x, y, z, m));
    }

    protected override void AddLine(double x, double y, double? z, double? m)
    {
      base.AddLine(x, y, z, m);
      this.buildChain.LineTo(new GeometryPosition(x, y, z, m));
    }

    protected override void EndFigure()
    {
      base.EndFigure();
      this.buildChain.EndFigure();
    }

    protected override void EndGeo()
    {
      base.EndGeo();
      this.buildChain.EndGeometry();
    }
  }
}
