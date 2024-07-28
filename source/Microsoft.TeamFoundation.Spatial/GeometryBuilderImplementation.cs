// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryBuilderImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Spatial
{
  internal class GeometryBuilderImplementation : GeometryPipeline, IGeometryProvider
  {
    private readonly SpatialTreeBuilder<Geometry> builder;

    public GeometryBuilderImplementation(SpatialImplementation creator) => this.builder = (SpatialTreeBuilder<Geometry>) new GeometryBuilderImplementation.GeometryTreeBuilder(creator);

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
    public event Action<Geometry> ProduceGeometry
    {
      add => this.builder.ProduceInstance += value;
      remove => this.builder.ProduceInstance -= value;
    }

    public Geometry ConstructedGeometry => this.builder.ConstructedInstance;

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
    public override void LineTo(GeometryPosition position) => this.builder.LineTo(position.X, position.Y, position.Z, position.M);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
    public override void BeginFigure(GeometryPosition position) => this.builder.BeginFigure(position.X, position.Y, position.Z, position.M);

    public override void BeginGeometry(SpatialType type) => this.builder.BeginGeo(type);

    public override void EndFigure() => this.builder.EndFigure();

    public override void EndGeometry() => this.builder.EndGeo();

    public override void Reset() => this.builder.Reset();

    public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
    {
      Util.CheckArgumentNull((object) coordinateSystem, nameof (coordinateSystem));
      this.builder.SetCoordinateSystem(coordinateSystem.EpsgId);
    }

    private class GeometryTreeBuilder : SpatialTreeBuilder<Geometry>
    {
      private readonly SpatialImplementation creator;
      private CoordinateSystem buildCoordinateSystem;

      public GeometryTreeBuilder(SpatialImplementation creator)
      {
        Util.CheckArgumentNull((object) creator, nameof (creator));
        this.creator = creator;
      }

      internal override void SetCoordinateSystem(int? epsgId) => this.buildCoordinateSystem = CoordinateSystem.Geometry(epsgId);

      protected override Geometry CreatePoint(
        bool isEmpty,
        double x,
        double y,
        double? z,
        double? m)
      {
        return !isEmpty ? (Geometry) new GeometryPointImplementation(this.buildCoordinateSystem, this.creator, x, y, z, m) : (Geometry) new GeometryPointImplementation(this.buildCoordinateSystem, this.creator);
      }

      protected override Geometry CreateShapeInstance(
        SpatialType type,
        IEnumerable<Geometry> spatialData)
      {
        switch (type)
        {
          case SpatialType.LineString:
            return (Geometry) new GeometryLineStringImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryPoint>().ToArray<GeometryPoint>());
          case SpatialType.Polygon:
            return (Geometry) new GeometryPolygonImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryLineString>().ToArray<GeometryLineString>());
          case SpatialType.MultiPoint:
            return (Geometry) new GeometryMultiPointImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryPoint>().ToArray<GeometryPoint>());
          case SpatialType.MultiLineString:
            return (Geometry) new GeometryMultiLineStringImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryLineString>().ToArray<GeometryLineString>());
          case SpatialType.MultiPolygon:
            return (Geometry) new GeometryMultiPolygonImplementation(this.buildCoordinateSystem, this.creator, spatialData.Cast<GeometryPolygon>().ToArray<GeometryPolygon>());
          case SpatialType.Collection:
            return (Geometry) new GeometryCollectionImplementation(this.buildCoordinateSystem, this.creator, spatialData.ToArray<Geometry>());
          default:
            return (Geometry) null;
        }
      }
    }
  }
}
