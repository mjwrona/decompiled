// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryPoint
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public abstract class GeometryPoint : Geometry
  {
    protected GeometryPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "X is meaningful")]
    public abstract double X { get; }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Y is meaningful")]
    public abstract double Y { get; }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
    public abstract double? Z { get; }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "m is meaningful")]
    public abstract double? M { get; }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x and y are meaningful")]
    public static GeometryPoint Create(double x, double y) => GeometryPoint.Create(CoordinateSystem.DefaultGeometry, x, y, new double?(), new double?());

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y and z are meaningful")]
    public static GeometryPoint Create(double x, double y, double? z) => GeometryPoint.Create(CoordinateSystem.DefaultGeometry, x, y, z, new double?());

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryPoint Create(double x, double y, double? z, double? m) => GeometryPoint.Create(CoordinateSystem.DefaultGeometry, x, y, z, m);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    public static GeometryPoint Create(
      CoordinateSystem coordinateSystem,
      double x,
      double y,
      double? z,
      double? m)
    {
      SpatialBuilder spatialBuilder = SpatialBuilder.Create();
      GeometryPipeline geometryPipeline = spatialBuilder.GeometryPipeline;
      geometryPipeline.SetCoordinateSystem(coordinateSystem);
      geometryPipeline.BeginGeometry(SpatialType.Point);
      geometryPipeline.BeginFigure(new GeometryPosition(x, y, z, m));
      geometryPipeline.EndFigure();
      geometryPipeline.EndGeometry();
      return (GeometryPoint) spatialBuilder.ConstructedGeometry;
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
    public bool Equals(GeometryPoint other)
    {
      bool? nullable1 = this.BaseEquals((Geometry) other);
      if (nullable1.HasValue)
        return nullable1.GetValueOrDefault();
      if (this.X == other.X && this.Y == other.Y)
      {
        double? nullable2 = this.Z;
        double? z = other.Z;
        if (nullable2.GetValueOrDefault() == z.GetValueOrDefault() & nullable2.HasValue == z.HasValue)
        {
          double? m = this.M;
          nullable2 = other.M;
          return m.GetValueOrDefault() == nullable2.GetValueOrDefault() & m.HasValue == nullable2.HasValue;
        }
      }
      return false;
    }

    public override bool Equals(object obj) => this.Equals(obj as GeometryPoint);

    public override int GetHashCode()
    {
      CoordinateSystem coordinateSystem = this.CoordinateSystem;
      double[] fields = new double[4]
      {
        this.IsEmpty ? 0.0 : this.X,
        this.IsEmpty ? 0.0 : this.Y,
        0.0,
        0.0
      };
      double? nullable = this.Z;
      fields[2] = nullable ?? 0.0;
      nullable = this.M;
      fields[3] = nullable ?? 0.0;
      return Geography.ComputeHashCodeFor<double>(coordinateSystem, (IEnumerable<double>) fields);
    }
  }
}
