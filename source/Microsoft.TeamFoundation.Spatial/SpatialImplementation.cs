// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public abstract class SpatialImplementation
  {
    private static SpatialImplementation spatialImplementation = (SpatialImplementation) new DataServicesSpatialImplementation();

    public static SpatialImplementation CurrentImplementation
    {
      get => SpatialImplementation.spatialImplementation;
      internal set => SpatialImplementation.spatialImplementation = value;
    }

    public abstract SpatialOperations Operations { get; set; }

    public abstract SpatialBuilder CreateBuilder();

    public abstract GeoJsonObjectFormatter CreateGeoJsonObjectFormatter();

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the common name for this format")]
    public abstract GmlFormatter CreateGmlFormatter();

    public abstract WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter();

    public abstract WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter(
      bool allowOnlyTwoDimensions);

    public abstract SpatialPipeline CreateValidator();

    internal SpatialOperations VerifyAndGetNonNullOperations() => this.Operations ?? throw new NotImplementedException(Strings.SpatialImplementation_NoRegisteredOperations);
  }
}
