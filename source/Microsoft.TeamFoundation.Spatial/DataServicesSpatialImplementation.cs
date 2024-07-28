// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.DataServicesSpatialImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal class DataServicesSpatialImplementation : SpatialImplementation
  {
    public override SpatialOperations Operations { get; set; }

    public override SpatialBuilder CreateBuilder()
    {
      GeographyBuilderImplementation builderImplementation1 = new GeographyBuilderImplementation((SpatialImplementation) this);
      GeometryBuilderImplementation builderImplementation2 = new GeometryBuilderImplementation((SpatialImplementation) this);
      ForwardingSegment forwardingSegment = new ForwardingSegment((GeographyPipeline) builderImplementation1, (GeometryPipeline) builderImplementation2);
      return new SpatialBuilder((GeographyPipeline) (SpatialPipeline) forwardingSegment, (GeometryPipeline) (SpatialPipeline) forwardingSegment, (IGeographyProvider) builderImplementation1, (IGeometryProvider) builderImplementation2);
    }

    public override GmlFormatter CreateGmlFormatter() => (GmlFormatter) new GmlFormatterImplementation((SpatialImplementation) this);

    public override GeoJsonObjectFormatter CreateGeoJsonObjectFormatter() => (GeoJsonObjectFormatter) new GeoJsonObjectFormatterImplementation((SpatialImplementation) this);

    public override WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter() => (WellKnownTextSqlFormatter) new WellKnownTextSqlFormatterImplementation((SpatialImplementation) this);

    public override WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter(
      bool allowOnlyTwoDimensions)
    {
      return (WellKnownTextSqlFormatter) new WellKnownTextSqlFormatterImplementation((SpatialImplementation) this, allowOnlyTwoDimensions);
    }

    public override SpatialPipeline CreateValidator() => (SpatialPipeline) new ForwardingSegment((SpatialPipeline) new SpatialValidatorImplementation());
  }
}
