// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialPipeline
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public class SpatialPipeline
  {
    private GeographyPipeline geographyPipeline;
    private GeometryPipeline geometryPipeline;
    private SpatialPipeline startingLink;

    public SpatialPipeline() => this.startingLink = this;

    public SpatialPipeline(GeographyPipeline geographyPipeline, GeometryPipeline geometryPipeline)
    {
      this.geographyPipeline = geographyPipeline;
      this.geometryPipeline = geometryPipeline;
      this.startingLink = this;
    }

    public virtual GeographyPipeline GeographyPipeline => this.geographyPipeline;

    public virtual GeometryPipeline GeometryPipeline => this.geometryPipeline;

    public SpatialPipeline StartingLink
    {
      get => this.startingLink;
      set => this.startingLink = value;
    }

    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "we have DrawGeometry and DrawGeography properties that can be used instead of the implicit cast")]
    public static implicit operator GeographyPipeline(SpatialPipeline spatialPipeline) => spatialPipeline?.GeographyPipeline;

    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "we have DrawGeometry and DrawGeography properties that can be used instead of the implicit cast")]
    public static implicit operator GeometryPipeline(SpatialPipeline spatialPipeline) => spatialPipeline?.GeometryPipeline;

    public virtual SpatialPipeline ChainTo(SpatialPipeline destination) => throw new NotImplementedException();
  }
}
