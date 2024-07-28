// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryCollectionImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeometryCollectionImplementation : GeometryCollection
  {
    private Geometry[] geometryArray;

    internal GeometryCollectionImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params Geometry[] geometry)
      : base(coordinateSystem, creator)
    {
      this.geometryArray = geometry ?? new Geometry[0];
    }

    internal GeometryCollectionImplementation(
      SpatialImplementation creator,
      params Geometry[] geometry)
      : this(CoordinateSystem.DefaultGeometry, creator, geometry)
    {
    }

    public override bool IsEmpty => this.geometryArray.Length == 0;

    public override ReadOnlyCollection<Geometry> Geometries => new ReadOnlyCollection<Geometry>((IList<Geometry>) this.geometryArray);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeometryPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeometry(SpatialType.Collection);
      for (int index = 0; index < this.geometryArray.Length; ++index)
        this.geometryArray[index].SendTo(pipeline);
      pipeline.EndGeometry();
    }
  }
}
