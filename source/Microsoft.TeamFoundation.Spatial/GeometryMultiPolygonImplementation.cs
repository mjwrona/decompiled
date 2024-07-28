// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryMultiPolygonImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeometryMultiPolygonImplementation : GeometryMultiPolygon
  {
    private GeometryPolygon[] polygons;

    internal GeometryMultiPolygonImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeometryPolygon[] polygons)
      : base(coordinateSystem, creator)
    {
      this.polygons = polygons;
    }

    internal GeometryMultiPolygonImplementation(
      SpatialImplementation creator,
      params GeometryPolygon[] polygons)
      : this(CoordinateSystem.DefaultGeometry, creator, polygons)
    {
    }

    public override bool IsEmpty => this.polygons.Length == 0;

    public override ReadOnlyCollection<Geometry> Geometries => new ReadOnlyCollection<Geometry>((IList<Geometry>) this.polygons);

    public override ReadOnlyCollection<GeometryPolygon> Polygons => new ReadOnlyCollection<GeometryPolygon>((IList<GeometryPolygon>) this.polygons);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeometryPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeometry(SpatialType.MultiPolygon);
      for (int index = 0; index < this.polygons.Length; ++index)
        this.polygons[index].SendTo(pipeline);
      pipeline.EndGeometry();
    }
  }
}
