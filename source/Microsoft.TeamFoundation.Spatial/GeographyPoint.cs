// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyPoint
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public abstract class GeographyPoint : Geography
  {
    protected GeographyPoint(CoordinateSystem coordinateSystem, SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
    }

    public abstract double Latitude { get; }

    public abstract double Longitude { get; }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
    public abstract double? Z { get; }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "m is meaningful")]
    public abstract double? M { get; }

    public static GeographyPoint Create(double latitude, double longitude) => GeographyPoint.Create(CoordinateSystem.DefaultGeography, latitude, longitude, new double?(), new double?());

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z is meaningful")]
    public static GeographyPoint Create(double latitude, double longitude, double? z) => GeographyPoint.Create(CoordinateSystem.DefaultGeography, latitude, longitude, z, new double?());

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    public static GeographyPoint Create(double latitude, double longitude, double? z, double? m) => GeographyPoint.Create(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    public static GeographyPoint Create(
      CoordinateSystem coordinateSystem,
      double latitude,
      double longitude,
      double? z,
      double? m)
    {
      SpatialBuilder spatialBuilder = SpatialBuilder.Create();
      GeographyPipeline geographyPipeline = spatialBuilder.GeographyPipeline;
      geographyPipeline.SetCoordinateSystem(coordinateSystem);
      geographyPipeline.BeginGeography(SpatialType.Point);
      geographyPipeline.BeginFigure(new GeographyPosition(latitude, longitude, z, m));
      geographyPipeline.EndFigure();
      geographyPipeline.EndGeography();
      return (GeographyPoint) spatialBuilder.ConstructedGeography;
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
    public bool Equals(GeographyPoint other)
    {
      bool? nullable1 = this.BaseEquals((Geography) other);
      if (nullable1.HasValue)
        return nullable1.GetValueOrDefault();
      if (this.Latitude == other.Latitude && this.Longitude == other.Longitude)
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

    public override bool Equals(object obj) => this.Equals(obj as GeographyPoint);

    public override int GetHashCode()
    {
      CoordinateSystem coordinateSystem = this.CoordinateSystem;
      double[] fields = new double[4]
      {
        this.IsEmpty ? 0.0 : this.Latitude,
        this.IsEmpty ? 0.0 : this.Longitude,
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
