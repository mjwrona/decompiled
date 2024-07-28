// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryLineStringImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeometryLineStringImplementation : GeometryLineString
  {
    private GeometryPoint[] points;

    internal GeometryLineStringImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeometryPoint[] points)
      : base(coordinateSystem, creator)
    {
      this.points = points ?? new GeometryPoint[0];
    }

    internal GeometryLineStringImplementation(
      SpatialImplementation creator,
      params GeometryPoint[] points)
      : this(CoordinateSystem.DefaultGeometry, creator, points)
    {
    }

    public override bool IsEmpty => this.points.Length == 0;

    public override ReadOnlyCollection<GeometryPoint> Points => new ReadOnlyCollection<GeometryPoint>((IList<GeometryPoint>) this.points);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeometryPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeometry(SpatialType.LineString);
      this.SendFigure(pipeline);
      pipeline.EndGeometry();
    }
  }
}
