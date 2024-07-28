// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyMultiPolygonImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyMultiPolygonImplementation : GeographyMultiPolygon
  {
    private GeographyPolygon[] polygons;

    internal GeographyMultiPolygonImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeographyPolygon[] polygons)
      : base(coordinateSystem, creator)
    {
      this.polygons = polygons;
    }

    internal GeographyMultiPolygonImplementation(
      SpatialImplementation creator,
      params GeographyPolygon[] polygons)
      : this(CoordinateSystem.DefaultGeography, creator, polygons)
    {
    }

    public override bool IsEmpty => this.polygons.Length == 0;

    public override ReadOnlyCollection<Geography> Geographies => new ReadOnlyCollection<Geography>((IList<Geography>) this.polygons);

    public override ReadOnlyCollection<GeographyPolygon> Polygons => new ReadOnlyCollection<GeographyPolygon>((IList<GeographyPolygon>) this.polygons);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.MultiPolygon);
      for (int index = 0; index < this.polygons.Length; ++index)
        this.polygons[index].SendTo(pipeline);
      pipeline.EndGeography();
    }
  }
}
