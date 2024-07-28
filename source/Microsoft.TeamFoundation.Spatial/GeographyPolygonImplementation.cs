// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyPolygonImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyPolygonImplementation : GeographyPolygon
  {
    private GeographyLineString[] rings;

    internal GeographyPolygonImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeographyLineString[] rings)
      : base(coordinateSystem, creator)
    {
      this.rings = rings ?? new GeographyLineString[0];
    }

    internal GeographyPolygonImplementation(
      SpatialImplementation creator,
      params GeographyLineString[] rings)
      : this(CoordinateSystem.DefaultGeography, creator, rings)
    {
    }

    public override bool IsEmpty => this.rings.Length == 0;

    public override ReadOnlyCollection<GeographyLineString> Rings => new ReadOnlyCollection<GeographyLineString>((IList<GeographyLineString>) this.rings);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.Polygon);
      for (int index = 0; index < this.rings.Length; ++index)
        this.rings[index].SendFigure(pipeline);
      pipeline.EndGeography();
    }
  }
}
