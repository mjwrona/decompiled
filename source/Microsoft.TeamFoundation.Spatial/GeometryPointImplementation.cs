// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryPointImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeometryPointImplementation : GeometryPoint
  {
    private double x;
    private double y;
    private double? z;
    private double? m;

    internal GeometryPointImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
      this.x = double.NaN;
      this.y = double.NaN;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "zvalue and mvalue are spelled correctly")]
    internal GeometryPointImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      double x,
      double y,
      double? z,
      double? m)
      : base(coordinateSystem, creator)
    {
      if (double.IsNaN(x) || double.IsInfinity(x))
        throw new ArgumentException(Strings.InvalidPointCoordinate((object) x, (object) nameof (x)));
      if (double.IsNaN(y) || double.IsInfinity(y))
        throw new ArgumentException(Strings.InvalidPointCoordinate((object) y, (object) nameof (y)));
      this.x = x;
      this.y = y;
      this.z = z;
      this.m = m;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "X is meaningful")]
    public override double X
    {
      get
      {
        if (this.IsEmpty)
          throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
        return this.x;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Y is meaningful")]
    public override double Y
    {
      get
      {
        if (this.IsEmpty)
          throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
        return this.y;
      }
    }

    public override bool IsEmpty => double.IsNaN(this.x);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    public override double? Z => this.z;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    public override double? M => this.m;

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeometryPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeometry(SpatialType.Point);
      if (!this.IsEmpty)
      {
        pipeline.BeginFigure(new GeometryPosition(this.x, this.y, this.z, this.m));
        pipeline.EndFigure();
      }
      pipeline.EndGeometry();
    }
  }
}
