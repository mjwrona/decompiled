// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyPointImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyPointImplementation : GeographyPoint
  {
    private double latitude;
    private double longitude;
    private double? z;
    private double? m;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "zvalue and mvalue are spelled correctly")]
    internal GeographyPointImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      double latitude,
      double longitude,
      double? zvalue,
      double? mvalue)
      : base(coordinateSystem, creator)
    {
      if (double.IsNaN(latitude) || double.IsInfinity(latitude))
        throw new ArgumentException(Strings.InvalidPointCoordinate((object) latitude, (object) nameof (latitude)));
      if (double.IsNaN(longitude) || double.IsInfinity(longitude))
        throw new ArgumentException(Strings.InvalidPointCoordinate((object) longitude, (object) nameof (longitude)));
      this.latitude = latitude;
      this.longitude = longitude;
      this.z = zvalue;
      this.m = mvalue;
    }

    internal GeographyPointImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
      this.latitude = double.NaN;
      this.longitude = double.NaN;
    }

    public override double Latitude
    {
      get
      {
        if (this.IsEmpty)
          throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
        return this.latitude;
      }
    }

    public override double Longitude
    {
      get
      {
        if (this.IsEmpty)
          throw new NotSupportedException(Strings.Point_AccessCoordinateWhenEmpty);
        return this.longitude;
      }
    }

    public override bool IsEmpty => double.IsNaN(this.latitude);

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    public override double? Z => this.z;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    public override double? M => this.m;

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.Point);
      if (!this.IsEmpty)
      {
        pipeline.BeginFigure(new GeographyPosition(this.latitude, this.longitude, this.z, this.m));
        pipeline.EndFigure();
      }
      pipeline.EndGeography();
    }
  }
}
