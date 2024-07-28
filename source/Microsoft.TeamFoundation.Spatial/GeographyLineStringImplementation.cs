// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyLineStringImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyLineStringImplementation : GeographyLineString
  {
    private GeographyPoint[] points;

    internal GeographyLineStringImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeographyPoint[] points)
      : base(coordinateSystem, creator)
    {
      this.points = points ?? new GeographyPoint[0];
    }

    public override bool IsEmpty => this.points.Length == 0;

    public override ReadOnlyCollection<GeographyPoint> Points => new ReadOnlyCollection<GeographyPoint>((IList<GeographyPoint>) this.points);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.LineString);
      this.SendFigure(pipeline);
      pipeline.EndGeography();
    }
  }
}
