// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialBuilder
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public class SpatialBuilder : 
    SpatialPipeline,
    IShapeProvider,
    IGeographyProvider,
    IGeometryProvider
  {
    private readonly IGeographyProvider geographyOutput;
    private readonly IGeometryProvider geometryOutput;

    public SpatialBuilder(
      GeographyPipeline geographyInput,
      GeometryPipeline geometryInput,
      IGeographyProvider geographyOutput,
      IGeometryProvider geometryOutput)
      : base(geographyInput, geometryInput)
    {
      this.geographyOutput = geographyOutput;
      this.geometryOutput = geometryOutput;
    }

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
    public event Action<Geography> ProduceGeography
    {
      add => this.geographyOutput.ProduceGeography += value;
      remove => this.geographyOutput.ProduceGeography -= value;
    }

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
    public event Action<Geometry> ProduceGeometry
    {
      add => this.geometryOutput.ProduceGeometry += value;
      remove => this.geometryOutput.ProduceGeometry -= value;
    }

    public Geography ConstructedGeography => this.geographyOutput.ConstructedGeography;

    public Geometry ConstructedGeometry => this.geometryOutput.ConstructedGeometry;

    public static SpatialBuilder Create() => SpatialImplementation.CurrentImplementation.CreateBuilder();
  }
}
