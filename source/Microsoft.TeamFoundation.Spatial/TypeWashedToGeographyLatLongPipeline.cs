// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.TypeWashedToGeographyLatLongPipeline
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal class TypeWashedToGeographyLatLongPipeline : TypeWashedPipeline
  {
    private readonly GeographyPipeline output;

    public TypeWashedToGeographyLatLongPipeline(SpatialPipeline output) => this.output = (GeographyPipeline) output;

    public override bool IsGeography => true;

    internal override void SetCoordinateSystem(int? epsgId) => this.output.SetCoordinateSystem(CoordinateSystem.Geography(epsgId));

    internal override void Reset() => this.output.Reset();

    internal override void BeginGeo(SpatialType type) => this.output.BeginGeography(type);

    internal override void BeginFigure(
      double coordinate1,
      double coordinate2,
      double? coordinate3,
      double? coordinate4)
    {
      this.output.BeginFigure(new GeographyPosition(coordinate1, coordinate2, coordinate3, coordinate4));
    }

    internal override void LineTo(
      double coordinate1,
      double coordinate2,
      double? coordinate3,
      double? coordinate4)
    {
      this.output.LineTo(new GeographyPosition(coordinate1, coordinate2, coordinate3, coordinate4));
    }

    internal override void EndFigure() => this.output.EndFigure();

    internal override void EndGeo() => this.output.EndGeography();
  }
}
