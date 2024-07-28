// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyBuilderImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Spatial
{
  internal class GeographyBuilderImplementation : GeographyPipeline, IGeographyProvider
  {
    private readonly SpatialTreeBuilder<Geography> builder;

    public GeographyBuilderImplementation(SpatialImplementation creator) => this.builder = (SpatialTreeBuilder<Geography>) new GeographyBuilderImplementation.GeographyTreeBuilder(creator);

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
    public event Action<Geography> ProduceGeography
    {
      add => this.builder.ProduceInstance += value;
      remove => this.builder.ProduceInstance -= value;
    }

    public Geography ConstructedGeography => this.builder.ConstructedInstance;

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
    public override void LineTo(GeographyPosition position) => this.builder.LineTo(position.Latitude, position.Longitude, position.Z, position.M);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "ForwardingSegment does the validation")]
    public override void BeginFigure(GeographyPosition position) => this.builder.BeginFigure(position.Latitude, position.Longitude, position.Z, position.M);

    public override void BeginGeography(SpatialType type) => this.builder.BeginGeo(type);

    public override void EndFigure() => this.builder.EndFigure();

    public override void EndGeography() => this.builder.EndGeo();

    public override void Reset() => this.builder.Reset();

    public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
    {
      Util.CheckArgumentNull((object) coordinateSystem, nameof (coordinateSystem));
      this.builder.SetCoordinateSystem(coordinateSystem.EpsgId);
    }

    private class GeographyTreeBuilder : SpatialTreeBuilder<Geography>
    {
      private readonly SpatialImplementation creator;
      private CoordinateSystem currentCoordinateSystem;

      public GeographyTreeBuilder(SpatialImplementation creator)
      {
        Util.CheckArgumentNull((object) creator, nameof (creator));
        this.creator = creator;
      }

      internal override void SetCoordinateSystem(int? epsgId) => this.currentCoordinateSystem = CoordinateSystem.Geography(epsgId);

      protected override Geography CreatePoint(
        bool isEmpty,
        double x,
        double y,
        double? z,
        double? m)
      {
        return !isEmpty ? (Geography) new GeographyPointImplementation(this.currentCoordinateSystem, this.creator, x, y, z, m) : (Geography) new GeographyPointImplementation(this.currentCoordinateSystem, this.creator);
      }

      protected override Geography CreateShapeInstance(
        SpatialType type,
        IEnumerable<Geography> spatialData)
      {
        switch (type)
        {
          case SpatialType.LineString:
            return (Geography) new GeographyLineStringImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyPoint>().ToArray<GeographyPoint>());
          case SpatialType.Polygon:
            return (Geography) new GeographyPolygonImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyLineString>().ToArray<GeographyLineString>());
          case SpatialType.MultiPoint:
            return (Geography) new GeographyMultiPointImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyPoint>().ToArray<GeographyPoint>());
          case SpatialType.MultiLineString:
            return (Geography) new GeographyMultiLineStringImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyLineString>().ToArray<GeographyLineString>());
          case SpatialType.MultiPolygon:
            return (Geography) new GeographyMultiPolygonImplementation(this.currentCoordinateSystem, this.creator, spatialData.Cast<GeographyPolygon>().ToArray<GeographyPolygon>());
          case SpatialType.Collection:
            return (Geography) new GeographyCollectionImplementation(this.currentCoordinateSystem, this.creator, spatialData.ToArray<Geography>());
          case SpatialType.FullGlobe:
            return (Geography) new GeographyFullGlobeImplementation(this.currentCoordinateSystem, this.creator);
          default:
            return (Geography) null;
        }
      }
    }
  }
}
